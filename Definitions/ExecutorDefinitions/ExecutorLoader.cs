using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace HelloWorldAPIProject.Definitions.ExecutorDefinitions
{
    public class ExecutorLoader
    {
        private readonly string _configPath;
        private readonly Dictionary<string, BashExecutorConfiguration> _executorConfigurations;

        public ExecutorLoader(string configPath)
        {
            _configPath = configPath;
            _executorConfigurations = new Dictionary<string, BashExecutorConfiguration>(StringComparer.OrdinalIgnoreCase);
            Console.WriteLine($"ExecutorLoader initialized with configPath: {_configPath}");
        }

        public void LoadConfigurations()
        {
            var executorDirectories = Directory.GetDirectories(Path.Combine(_configPath, "executors"));

            foreach (var dir in executorDirectories)
            {
                Console.WriteLine($"Loading configuration from directory: {dir}");
                var configuration = JsonSerializer.Deserialize<BashExecutorConfiguration>(File.ReadAllText(Path.Combine(dir, "executor.json")), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                _executorConfigurations.Add(Path.GetFileName(dir), configuration);
            }
        }

        public BashExecutorConfiguration GetExecutorConfiguration(string executor)
        {
            _executorConfigurations.TryGetValue(executor, out var configuration);
            return configuration;
        }
    }
}