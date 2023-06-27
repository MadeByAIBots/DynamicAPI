using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DynamicApiServer.Definitions.ExecutorDefinitions
{
    public class ExecutorDefinitionLoader
    {
        private readonly string _configPath;
        private readonly Dictionary<string, BashExecutorDefinition> _executorConfigurations;

        public ExecutorDefinitionLoader(string configPath)
        {
            _configPath = configPath;
            _executorConfigurations = new Dictionary<string, BashExecutorDefinition>(StringComparer.OrdinalIgnoreCase);
            Console.WriteLine($"ExecutorDefinitionLoader initialized with configPath: {_configPath}");
        }

        public void LoadConfigurations()
        {
            var executorDirectories = Directory.GetDirectories(Path.Combine(_configPath, "executors"));

            foreach (var dir in executorDirectories)
            {
                Console.WriteLine($"Loading configuration from directory: {dir}");
                var configuration = JsonSerializer.Deserialize<BashExecutorDefinition>(File.ReadAllText(Path.Combine(dir, "executor.json")), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                _executorConfigurations.Add(Path.GetFileName(dir), configuration);
            }
        }

        public BashExecutorDefinition GetExecutorDefinition(string executor)
        {
            _executorConfigurations.TryGetValue(executor, out var configuration);
            return configuration;
        }
    }
}