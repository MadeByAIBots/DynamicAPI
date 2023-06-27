using HelloWorldAPIProject.Definitions.EndpointDefinitions;
using HelloWorldAPIProject.Definitions.ExecutorDefinitions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class EndpointService
{
    private readonly string _configPath;
    private readonly Dictionary<string, BashExecutorConfiguration> _executorConfigurations;

    public EndpointService(string configPath)
    {
        _configPath = configPath;
        _executorConfigurations = new Dictionary<string, BashExecutorConfiguration>(StringComparer.OrdinalIgnoreCase);
        Console.WriteLine($"[INFO] Initializing EndpointService with configPath: {_configPath}");
    }

    public List<EndpointConfiguration> LoadConfigurations()
    {
        var configurations = new List<EndpointConfiguration>();
        var endpointDirectories = Directory.GetDirectories(_configPath);
        Console.WriteLine($"[INFO] Found {endpointDirectories.Length} endpoint directories to load configurations from.");

        foreach (var dir in endpointDirectories)
        {
            var endpointName = Path.GetFileName(dir);
            Console.WriteLine($"[INFO] Loading configuration for endpoint: {endpointName}");

            var configuration = JsonSerializer.Deserialize<EndpointConfiguration>(File.ReadAllText(Path.Combine(dir, "endpoint.json")), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            configurations.Add(configuration);

            Console.WriteLine($"[INFO] Executor configuration would be loaded here for executor: {configuration.Executor}");
            if (configuration.Executor == "bash")
            {
                var executorConfiguration = JsonSerializer.Deserialize<BashExecutorConfiguration>(File.ReadAllText(Path.Combine(dir, configuration.Executor + ".json")), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (!_executorConfigurations.ContainsKey(configuration.Executor))
                _executorConfigurations.Add(configuration.Executor, executorConfiguration);
            }
            else
            {
                Console.WriteLine($"[INFO] Executor type {configuration.Executor} not supported.");
            }

            Console.WriteLine($"[INFO] Successfully loaded configurations for {endpointName} endpoint.");
        }

        return configurations;
    }

    public BashExecutorConfiguration GetExecutorConfiguration(string executor)
    {
        Console.WriteLine($"[INFO] Executor configuration would be retrieved here for executor: {executor}");
        if (_executorConfigurations.ContainsKey(executor))
        {
            return _executorConfigurations[executor];
        }
        else
        {
            Console.WriteLine($"[INFO] Executor type {executor} not supported.");
            return null;
        }
    }

    public EndpointConfiguration GetEndpointConfiguration(string path)
    {
        foreach (var config in LoadConfigurations())
        {
            if (config.Path == path)
            {
                return config;
            }
        }

        return null;
    }
}