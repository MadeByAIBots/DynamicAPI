using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DynamicApi.Endpoints.Model;
using DynamicApi.Contracts;
using System.Reflection;
using Microsoft.CodeAnalysis.Emit;

namespace DynamicApiServer.Execution.Executors.CSharpScript
{
	public class CSharpScriptEndpointExecutor : IEndpointExecutor
	{
		private readonly ILogger<CSharpScriptEndpointExecutor> _logger;
		private readonly ILoggerFactory _loggerFactory;
		private readonly ApiConfiguration _apiConfiguration;
		private readonly WorkingDirectoryResolver _workingDirectoryResolver;
		private readonly CSharpScriptLocator _scriptLocator;
		private readonly CSharpScriptCompiler _scriptCompiler;
		private readonly CSharpScriptReflector _scriptReflector;
		private readonly CSharpScriptResultHandler _resultHandler;


		public CSharpScriptEndpointExecutor(
		ILoggerFactory loggerFactory,
		ApiConfiguration apiConfiguration,
		WorkingDirectoryResolver workingDirectoryResolver,
			CSharpScriptLocator scriptLocator,
			CSharpScriptCompiler scriptCompiler,
			CSharpScriptReflector scriptReflector,
			CSharpScriptResultHandler resultHandler)
		{
			_loggerFactory = loggerFactory;
			_logger = loggerFactory.CreateLogger<CSharpScriptEndpointExecutor>();
			_apiConfiguration = apiConfiguration;
			_workingDirectoryResolver = workingDirectoryResolver;
			_scriptLocator = scriptLocator;
			_scriptCompiler = scriptCompiler;
			_scriptReflector = scriptReflector;
			_resultHandler = resultHandler;
		}

		public async Task<string> ExecuteCommand(EndpointDefinition endpointDefinition, IExecutorDefinition executorConfig, Dictionary<string, string> args)
		{
			_logger.LogInformation("ExecuteCommand called with EndpointDefinition: {0}, ExecutorDefinition: {1}, args: {2}", endpointDefinition, executorConfig, args);

			try
			{
				var csharpExecutorConfig = (CSharpScriptExecutorDefinition)executorConfig;
				string scriptPath = _scriptLocator.LocateScript(csharpExecutorConfig.Script, endpointDefinition.FolderName);

				var scriptOptions = CreateScriptOptions();


				var script = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.Create(File.ReadAllText(scriptPath), scriptOptions);
				var result = await ProcessScript(script, args);

				return result.ToString();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to execute C# script.");
				return $"Error: {ex.Message}";
			}
		}

		private ScriptOptions CreateScriptOptions()
		{
			var scriptOptions = Microsoft.CodeAnalysis.Scripting.ScriptOptions.Default;

			var references = GetReferences();
			foreach (var reference in references)
			{
				scriptOptions = scriptOptions.AddReferences(reference);
			}


			var usings = GetUsings();
			foreach (var usingStatement in usings)
			{
				scriptOptions = scriptOptions.WithImports(usingStatement);
			}

			return scriptOptions;

		}

		private async Task<string> ProcessScript(Script script, Dictionary<string, string> endpointParameters)

		{
			var compilation = script.GetCompilation();

			using (var ms = new MemoryStream())
			{
				var emitResult = compilation.Emit(ms);
				if (!emitResult.Success)
				{
					return await HandleCompilationFailure(emitResult);
				}

				var assembly = _scriptReflector.GetAssemblyFromMemoryStream(ms);
				var type = _scriptReflector.GetTypeFromAssembly(assembly);
				var instance = _scriptReflector.GetClassInstanceFromType(type);
				var method = _scriptReflector.GetMethodFromType(type);

				var result = await InvokeMethodOnScript(instance, method, endpointParameters);
				string output = result.Body;

				_logger.LogInformation("Script executed. Output: {0}", output);

				return output;
			}

			return String.Empty;
		}

		private async Task<EndpointExecutionResult> InvokeMethodOnScript(object instance, MethodInfo method, Dictionary<string, string> endpointParameters)
		{
			_logger.LogInformation("Executing script method...");

			var invokeArguments = new object[] { new DynamicExecutionParameters(_apiConfiguration, _workingDirectoryResolver, _loggerFactory, endpointParameters) };

			return await (Task<EndpointExecutionResult>)method.Invoke(instance, invokeArguments);
		}

		private async Task<string> HandleCompilationFailure(EmitResult emitResult)
		{
			_logger.LogError("Compilation failed.");
			foreach (var diagnostic in emitResult.Diagnostics)
			{
				var location = diagnostic.Location;
				if (location.IsInSource)
				{
					var lineSpan = location.GetLineSpan(); // LineSpan includes the file path and the line number
					var fileName = lineSpan.Path;
					var lineNumber = lineSpan.StartLinePosition.Line;
					var errorMessage = diagnostic.GetMessage();

					_logger.LogError("Compilation failed in {0} at line {1}: {2}", fileName, lineNumber, errorMessage);
				}
				else
				{
					// If the location is not in source, it's a global issue, and we don't have a specific file or line number
					var errorMessage = diagnostic.GetMessage();
					_logger.LogError("Compilation failed: {0}", errorMessage);
				}
			}
			return "Error: Compilation failed.";
		}

		private List<string> GetReferences()
		{
			var references = new List<string>();

			_logger.LogInformation("Getting script references:");

			foreach (var reference in _apiConfiguration.CSharpScript.References)
			{
				if (IsAssemblyName(reference))
				{
					_logger.LogInformation(reference);
					references.Add(reference);
				}
				else if (IsFilePathWithWildcard(reference))
				{
					var resolvedDirectory = ResolvePath(reference);
					var matchingFiles = ListFiles(resolvedDirectory, GetWildcardPattern(reference));

					foreach (var matchingFile in matchingFiles)
					{
						_logger.LogInformation(matchingFile);
					}
					references.AddRange(matchingFiles);
				}
				else // IsFilePathWithoutWildcard(reference)
				{
					var resolvedPath = ResolvePath(reference);
					references.Add(resolvedPath);
				}
			}
			return references;
		}
		private bool IsAssemblyName(string reference)
		{
			return !reference.Contains('/') && !reference.Contains('*');
		}

		private bool IsFilePathWithWildcard(string reference)
		{
			return reference.Contains('*');
		}

		private List<string> ListFiles(string directory, string pattern)
		{
			return Directory.GetFiles(Path.GetDirectoryName(directory), Path.GetFileName(directory)).ToList();
		}

		private string GetWildcardPattern(string reference)
		{
			return Path.GetFileName(reference);
		}

		private string ResolvePath(string reference)
		{
			return Path.Combine(_workingDirectoryResolver.WorkingDirectory(), reference);
		}

		private List<string> GetUsings()
		{
			return _apiConfiguration.CSharpScript.Usings.ToList();
		}
	}
}
