using DynamicApiServer.Definitions.EndpointDefinitions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using DynamicApiConfiguration;

namespace DynamicApiServer;

public class EndpointLoader
{
    private readonly ApiConfiguration _config;
    private readonly WorkingDirectoryResolver _resolver;

    public EndpointLoader(ApiConfiguration config, WorkingDirectoryResolver resolver)
    {
        _config = config;
        _resolver = resolver;
        Console.WriteLine($"EndpointLoader initialized with configPath: {_config.EndpointPath}");
    }

    public List<EndpointDefinition> LoadConfigurations()
    {
        var configurations = new List<EndpointDefinition>();
        var endpointDirectories = Directory.GetDirectories(_resolver.WorkingDirectory() + "/" + _config.EndpointPath);

        foreach (var dir in endpointDirectories)
        {
            Console.WriteLine($"Loading configuration from directory: {dir}");
            var endpointFilePath = File.ReadAllText(Path.Combine(dir, "endpoint.json"));
            var configuration = JsonSerializer.Deserialize<EndpointDefinition>(endpointFilePath, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            configurations.Add(configuration);
            configuration.FolderName = Path.GetFileName(dir);

            Console.WriteLine($"Executor configuration would be loaded here for executor: {configuration.Executor}");
        }

        return configurations;
    }
}