using DynamicApiServer.Definitions.EndpointDefinitions;
using DynamicApiServer.Definitions.ExecutorDefinitions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DynamicApiServer;

public class EndpointService
{
    private readonly EndpointLoader _endpointLoader;
    private readonly ExecutorDefinitionLoader _executorLoader;

    public EndpointService(EndpointLoader endpointLoader, ExecutorDefinitionLoader executorLoader)
    {
        _endpointLoader = endpointLoader;
        _executorLoader = executorLoader;
        Console.WriteLine($"[INFO] Initializing EndpointService with EndpointLoader and ExecutorDefinitionLoader");
    }

    public IExecutorDefinition GetExecutorDefinition(EndpointDefinition endpointConfig)
    {
        Console.WriteLine($"[INFO] Executor configuration would be retrieved here for executor: {endpointConfig.Executor}");
        return _executorLoader.LoadConfiguration(endpointConfig);
    }

    public EndpointDefinition GetEndpointDefinition(string path)
    {
        foreach (var config in _endpointLoader.LoadConfigurations())
        {
            if (config.Path == path)
            {
                return config;
            }
        }

        return null;
    }
}