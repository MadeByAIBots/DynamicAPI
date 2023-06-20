using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class EndpointLoader
{
    private readonly string _configPath;

    public EndpointLoader(string configPath)
    {
        _configPath = configPath;
        Console.WriteLine($"EndpointLoader initialized with configPath: {_configPath}");
    }

    public List<EndpointConfiguration> LoadConfigurations()
    {
        var configurations = new List<EndpointConfiguration>();
        var endpointDirectories = Directory.GetDirectories(Path.Combine(_configPath, "endpoints"));

        foreach (var dir in endpointDirectories)
        {
            Console.WriteLine($"Loading configuration from directory: {dir}");
            var configuration = JsonSerializer.Deserialize<EndpointConfiguration>(File.ReadAllText(Path.Combine(dir, "endpoint.json")), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var executorCommand = JsonSerializer.Deserialize<ExecutorCommand>(File.ReadAllText(Path.Combine(dir, "bash.json")), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            configuration.Command = executorCommand.Command;
            configurations.Add(configuration);
        }

        return configurations;
    }
}

public class ExecutorCommand
{
    public string Command { get; set; }
}