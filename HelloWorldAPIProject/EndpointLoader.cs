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
        var endpointFiles = Directory.GetFiles(Path.Combine(_configPath, "endpoints"), "*.json");

        foreach (var file in endpointFiles)
        {
            Console.WriteLine($"Loading configuration from file: {file}");
            var configuration = JsonSerializer.Deserialize<EndpointConfiguration>(File.ReadAllText(file), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            configurations.Add(configuration);
        }

        return configurations;
    }
}