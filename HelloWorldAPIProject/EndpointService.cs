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
        Console.WriteLine($"EndpointService initialized with configPath: {_configPath}");
    }

    public List<EndpointConfiguration> LoadConfigurations()
    {
        var configurations = new List<EndpointConfiguration>();
        var endpointDirectories = Directory.GetDirectories(_configPath);

        foreach (var dir in endpointDirectories)
        {
            Console.WriteLine($"Loading configuration from directory: {dir}");
            var configuration = JsonSerializer.Deserialize<EndpointConfiguration>(File.ReadAllText(Path.Combine(dir, "endpoint.json")), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            configurations.Add(configuration);

            var executorConfiguration = JsonSerializer.Deserialize<BashExecutorConfiguration>(File.ReadAllText(Path.Combine(dir, configuration.Executor + ".json")), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            _executorConfigurations.Add(configuration.Executor, executorConfiguration);
        }

        return configurations;
    }

    public BashExecutorConfiguration GetExecutorConfiguration(string executor)
    {
        return _executorConfigurations[executor];
    }
}