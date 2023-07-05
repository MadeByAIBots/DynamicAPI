using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DynamicApiServer.Definitions.EndpointDefinitions;
using DynamicApiServer.Definitions.ExecutorDefinitions;
using DynamicApi.Contracts;
using DynamicApiConfiguration;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting.Hosting;
using System.Reflection;
using DynamicApi.Contracts;

namespace DynamicApiServer.Execution.Executors.CSharpScript
{
	public class CSharpScriptEndpointExecutor : IEndpointExecutor
	{
		private readonly ILogger<CSharpScriptEndpointExecutor> _logger;
		private readonly ILoggerFactory _loggerFactory;

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
			return Path.Combine(_resolver.WorkingDirectory(), reference);
		}
		private List<string> GetReferences()
		{
			var references = new List<string>();

			foreach (var reference in _apiConfig.CSharpScript.References)
			{
				if (IsAssemblyName(reference))
				{
					references.Add(reference);
				}
				else if (IsFilePathWithWildcard(reference))
				{
					var resolvedDirectory = ResolvePath(reference);
					var matchingFiles = ListFiles(resolvedDirectory, GetWildcardPattern(reference));

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
		private List<string> GetUsings()
		{
			return _apiConfig.CSharpScript.Usings;
		}
		private readonly ApiConfiguration _apiConfig;
		private readonly CSharpScriptUtilities _utilities;
		private readonly WorkingDirectoryResolver _resolver;

		public CSharpScriptEndpointExecutor(ApiConfiguration apiConfig, WorkingDirectoryResolver resolver, CSharpScriptUtilities utilities, ILoggerFactory loggerFactory)
		{
			_resolver = resolver;
			_utilities = utilities;
			_apiConfig = apiConfig;
			_loggerFactory = loggerFactory;
			_logger = loggerFactory.CreateLogger<CSharpScriptEndpointExecutor>();
			_logger.LogInformation("CSharpScriptEndpointExecutor initialized with ApiConfiguration: {0}", _apiConfig);
		}

		public async Task<string> ExecuteCommand(EndpointDefinition endpointDefinition, IExecutorDefinition executorConfig, Dictionary<string, string> args)
		{
			_logger.LogInformation("ExecuteCommand called with EndpointDefinition: {0}, ExecutorDefinition: {1}, args: {2}", endpointDefinition, executorConfig, args);

			try
			{
				_logger.LogInformation("Validating input...");
				_utilities.ValidateInput(executorConfig, args);
				_logger.LogInformation("Input validated.");

				_logger.LogInformation("Finding script...");
				var csharpScriptExecutorConfig = executorConfig as CSharpScriptExecutorDefinition;
				string scriptPath = _utilities.FindScript(csharpScriptExecutorConfig.Script, endpointDefinition.FolderName, _apiConfig);
				_logger.LogInformation("Script found at path: {0}", scriptPath);

				_logger.LogInformation("Validating script...");
				_utilities.ValidateScript(scriptPath);
				_logger.LogInformation("Script validated.");

				_logger.LogInformation("Compiling script...");
				string scriptCode = File.ReadAllText(scriptPath);

				var scriptOptions = Microsoft.CodeAnalysis.Scripting.ScriptOptions.Default;

				var references = GetReferences();
				foreach (var reference in references)
				{
					scriptOptions = scriptOptions.AddReferences(reference);
				}
				_logger.LogInformation("Adding references");



				var usings = GetUsings();
				foreach (var usingStatement in usings)
				{
					scriptOptions = scriptOptions.WithImports(usingStatement);
				}

				var script = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.Create(scriptCode, scriptOptions);
				var compilation = script.GetCompilation();

				using (var ms = new MemoryStream())
				{
					var emitResult = compilation.Emit(ms);
					if (!emitResult.Success)
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
					ms.Seek(0, SeekOrigin.Begin);
					var assembly = Assembly.Load(ms.ToArray());

					_logger.LogInformation("Listing all types in the assembly...");
					foreach (var t in assembly.GetTypes())
					{
						_logger.LogInformation("Found type: {0}", t.FullName);
					}

					_logger.LogInformation("Finding script class...");
					var type = assembly.GetTypes().FirstOrDefault(t => t.GetInterfaces().Contains(typeof(IDynamicEndpointExecutor)));
					if (type == null)
					{
						_logger.LogError("No class found that implements IDynamicEndpointExecutor.");
						return "Error: No class found that implements IDynamicEndpointExecutor.";
					}
					_logger.LogInformation("Script class found: {0}", type.Name);

					_logger.LogInformation("Instantiating script class...");
					var instance = Activator.CreateInstance(type);

					_logger.LogInformation("Executing script method...");
					var method = type.GetMethod("ExecuteAsync");
					var result = await (Task<EndpointExecutionResult>)method.Invoke(instance, new object[] { new DynamicExecutionParameters(_apiConfig, _resolver, _loggerFactory, args) });
					string output = result.Body;

					_logger.LogInformation("Script executed. Output: {0}", output);

					return output;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to execute C# script.");
				return $"Error: {ex.Message}";
			}
		}



	}
}
