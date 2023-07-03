using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using DynamicApiServer.Definitions.EndpointDefinitions;
using Microsoft.Extensions.Logging;

public class EndpointFileReader
{
    private readonly ILogger<EndpointFileReader> _logger;

    public EndpointFileReader(ILogger<EndpointFileReader> logger)
    {
        _logger = logger;
    }

    public List<EndpointDefinition> ReadEndpointFiles(string directory)
    {
        _logger.LogInformation($"Reading endpoint definition files from directory '{directory}'...");

        if (!Directory.Exists(directory))
        {
            var message = $"Directory '{directory}' does not exist.";
            _logger.LogError(message);
            throw new DirectoryNotFoundException(message);
        }

        var files = Directory.GetFiles(directory, "endpoint.json", SearchOption.AllDirectories);
        if (files.Length == 0)
        {
            var message = "No 'endpoint.json' files found.";
            _logger.LogError(message);
            throw new FileNotFoundException(message);
        }

        _logger.LogInformation($"Found {files.Length} endpoint definition files.");

        var endpointDefinitions = new List<EndpointDefinition>();
        foreach (var file in files)
        {
            _logger.LogInformation($"Reading endpoint definition file '{file}'...");

            var content = File.ReadAllText(file);
var settings = new JsonSerializerSettings { ContractResolver = new DefaultContractResolver { NamingStrategy = null } }; var endpointDefinition = JsonConvert.DeserializeObject<EndpointDefinition>(content, settings);
            if (endpointDefinition == null || string.IsNullOrEmpty(endpointDefinition.Method) || string.IsNullOrEmpty(endpointDefinition.Path))
            {
                var message = $"Invalid data in file '{file}'.";
                _logger.LogError(message);
                throw new Exception(message);
            }
            endpointDefinitions.Add(endpointDefinition);
        }

        _logger.LogInformation("Finished reading endpoint definition files.");

        return endpointDefinitions;
    }
}
