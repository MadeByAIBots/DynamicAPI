using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DynamicApiServer.Definitions.ExecutorDefinitions;
using DynamicApiServer.Definitions.EndpointDefinitions;
using DynamicApi.Contracts;
using DynamicApiConfiguration;

namespace DynamicApiServer.Execution.Executors.CSharp
{
    public class CSharpEndpointExecutor : IEndpointExecutor
    {
        private readonly ILogger<CSharpEndpointExecutor> _logger;
        private readonly ApiConfiguration _apiConfig;
        private readonly WorkingDirectoryResolver _resolver;

        public CSharpEndpointExecutor(ApiConfiguration apiConfig, WorkingDirectoryResolver resolver, ILoggerFactory loggerFactory)
        {
            _apiConfig = apiConfig;
            _resolver = resolver;
            _logger = loggerFactory.CreateLogger<CSharpEndpointExecutor>();
            _logger.LogInformation("CSharpEndpointExecutor initialized.");
        }

        public async Task<string> ExecuteCommand(EndpointDefinition endpointDefinition, IExecutorDefinition executorConfig, Dictionary<string, string> args)
        {
            ValidateInput(executorConfig, args);

            var csharpExecutorConfig = executorConfig as CSharpExecutorDefinition;

            string assemblyName = csharpExecutorConfig.Assembly;
            string className = csharpExecutorConfig.Class;

            string dllPath = FindDll(assemblyName, endpointDefinition.FolderName);
            Assembly assembly = LoadAssembly(dllPath);
            Type type = GetTypeFromAssembly(assembly, className);
            IDynamicEndpointExecutor endpointExecutor = CreateInstance(type);

            EndpointExecutionResult result = await ExecuteEndpoint(endpointExecutor, args);

            return result.Body;
        }

        private void ValidateInput(IExecutorDefinition executorConfig, Dictionary<string, string> args)
        {
            if (executorConfig == null)
            {
                _logger.LogError("Executor configuration is null. Cannot execute command without executor configuration.");
                throw new ArgumentNullException(nameof(executorConfig));
            }

            if (args == null)
            {
                _logger.LogError("Arguments dictionary is null. Cannot execute command without arguments.");
                throw new ArgumentNullException(nameof(args));
            }

            if (!(executorConfig is CSharpExecutorDefinition))
            {
                _logger.LogError($"Executor configuration is not of type CSharpExecutorConfig. Actual type: {executorConfig.GetType().Name}");
                throw new ArgumentException("Invalid executor configuration type.", nameof(executorConfig));
            }
        }

        private string FindDll(string assemblyName, string folderName)
        {
            string baseDirectory = Path.Combine(_resolver.WorkingDirectory(), _apiConfig.EndpointPath, folderName, "bin");

            _logger.LogInformation("Looking for assembly in: {baseDirectory}");

            var fullAssemblyName = assemblyName;
            if (!fullAssemblyName.EndsWith(".dll"))
                fullAssemblyName += ".dll";

            string dllPath = Directory.GetFiles(baseDirectory, fullAssemblyName, SearchOption.AllDirectories).FirstOrDefault();

            if (string.IsNullOrEmpty(dllPath))
            {
                _logger.LogError($"Could not find DLL '{assemblyName}.dll' in directory '{baseDirectory}'.");
                throw new FileNotFoundException($"Could not find DLL '{assemblyName}.dll'.", dllPath);
            }

            _logger.LogInformation($"Found DLL at '{dllPath}'.");
            return dllPath;
        }

        private Assembly LoadAssembly(string dllPath)
        {
            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFrom(dllPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to load assembly from '{dllPath}'.");
                throw;
            }

            _logger.LogInformation($"Successfully loaded assembly from '{dllPath}'.");
            return assembly;
        }

        private Type GetTypeFromAssembly(Assembly assembly, string className)
        {
            Type type = assembly.GetType(className, throwOnError: true);
            _logger.LogInformation($"Successfully retrieved type '{className}' from assembly.");
            return type;
        }

        private IDynamicEndpointExecutor CreateInstance(Type type)
        {
            if (!typeof(IDynamicEndpointExecutor).IsAssignableFrom(type))
            {
                _logger.LogError($"Type '{type.FullName}' does not implement IDynamicEndpointExecutor.");
                throw new ArgumentException($"Type '{type.FullName}' does not implement IDynamicEndpointExecutor.", nameof(type));
            }

            IDynamicEndpointExecutor instance;
            try
            {
                instance = (IDynamicEndpointExecutor)Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to create instance of type '{type.FullName}'.");
                throw;
            }

            _logger.LogInformation($"Successfully created instance of type '{type.FullName}'.");
            return instance;
        }

        private async Task<EndpointExecutionResult> ExecuteEndpoint(IDynamicEndpointExecutor endpointExecutor, Dictionary<string, string> args)
        {
            EndpointExecutionResult result;

            var parameters = new DynamicExecutionParameters(_apiConfig, _resolver, args);

            try
            {
                result = await endpointExecutor.ExecuteAsync(parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute endpoint.");
                throw;
            }

            _logger.LogInformation("Successfully executed endpoint.");
            return result;
        }
    }
}
