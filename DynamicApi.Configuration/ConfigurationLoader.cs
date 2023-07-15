using DynamicApi.WorkingDirectory;
using System;
using System.IO;
using System.Text.Json;

namespace DynamicApi.Configuration
{
    public class ConfigurationLoader
    {
        private readonly WorkingDirectoryResolver _workingDirectoryResolver;

        public ConfigurationLoader(WorkingDirectoryResolver workingDirectoryResolver)
        {
            _workingDirectoryResolver = workingDirectoryResolver;
        }

        public ApiConfiguration LoadConfiguration()
        {
            var configPath = _workingDirectoryResolver.WorkingDirectory() + "/config.json";
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
}