using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

public class EndpointLoader
{
    private string configPath;

    public EndpointLoader(string configPath)
    {
        this.configPath = configPath;
    }

    public List<EndpointConfiguration> LoadConfigurations()
    {
        Console.WriteLine($"Loading configurations from {configPath}/endpoints/bash_hello_world.json");

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            var endpointConfigurations = JsonSerializer.Deserialize<List<EndpointConfiguration>>(
                File.ReadAllText(configPath + "/endpoints/bash_hello_world.json"), options);

            Console.WriteLine($"Loaded {endpointConfigurations.Count} configurations successfully.");

            foreach (var config in endpointConfigurations)
            {
                Console.WriteLine($"Loaded configuration: Path = {config.Path}, Executor = {config.Executor}, Command = {config.Command}");
            }

            return endpointConfigurations;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error loading configurations: {e.Message}");
            return null;
        }
    }
}