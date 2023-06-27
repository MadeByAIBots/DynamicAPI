using System;
using System.IO;
using System.Text.Json;

namespace DynamicApiConfiguration;

public class ConfigurationLoader
{
    public ApiConfiguration LoadConfiguration(string configPath)
    {
        try
        {
            var configJson = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<ApiConfiguration>(configJson);
            return config;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to load configuration: {ex.Message}");
            return null;
        }
    }
}