using DynamicApi.WorkingDirectory;
using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace DynamicApi.Configuration
{
    public class ConfigurationLoader
    {
        private readonly WorkingDirectoryResolver _workingDirectoryResolver;
        private readonly IConfiguration _configuration;

        public ConfigurationLoader(WorkingDirectoryResolver workingDirectoryResolver)
        {
            _workingDirectoryResolver = workingDirectoryResolver;
            _configuration = new ConfigurationBuilder()
                .SetBasePath(_workingDirectoryResolver.WorkingDirectory())
                .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                .AddJsonFile("config.override.json", optional: true, reloadOnChange: true)
                .Build();
        }

        public ApiConfiguration LoadConfiguration()
        {
            var config = new ApiConfiguration();
            _configuration.Bind(config);
            return config;
        }
    }
}