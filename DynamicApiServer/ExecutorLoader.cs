using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using DynamicApiServer.Definitions.ExecutorDefinitions;
using DynamicApiServer.Definitions.EndpointDefinitions;
using DynamicApiConfiguration;

namespace DynamicApiServer
{
    public class ExecutorDefinitionLoader
    {
        private readonly ILogger _logger;
        private readonly ApiConfiguration _config;

        public ExecutorDefinitionLoader(ApiConfiguration config, ILoggerFactory loggerFactory)
        {
            _config = config;
            _logger = new Logger<ExecutorDefinitionLoader>(loggerFactory);
        }

        public IExecutorDefinition LoadConfiguration(EndpointDefinition endpointConfig)
        {
            try
            {                
                _logger.LogInformation($"Loading executor configuration...");
                _logger.LogInformation($"  Endpoints path: {_config.EndpointPath}");
                _logger.LogInformation($"  Endpoint path: {endpointConfig.Path}");
                _logger.LogInformation($"  Executor: {endpointConfig.Executor}");

                // Determine the path to the executor configuration file
                string configFilePath = _config.EndpointPath + "/" + endpointConfig.FolderName + "/" + endpointConfig.Executor + ".json";

                _logger.LogInformation($"  Executor path: {configFilePath}");

                // Read the configuration file
                string configJson = File.ReadAllText(configFilePath);

                // Deserialize the configuration into the appropriate object
                if (endpointConfig.Executor == "bash")
                {
                    _logger.LogInformation("Deserializing configuration into BashExecutorDefinition object");
                    return JsonSerializer.Deserialize<BashExecutorDefinition>(configJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                // Add additional deserialization logic for other executor types here...

                _logger.LogError("Unsupported executor type: " + endpointConfig.Executor);
                throw new Exception("Unsupported executor type: " + endpointConfig.Executor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading executor configuration");
                throw;
            }
        }
    }
}