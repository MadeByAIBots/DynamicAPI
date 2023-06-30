using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using DynamicApiServer.Definitions.EndpointDefinitions;
using Microsoft.Extensions.Logging;

public class ParameterProcessor
{
    private readonly ILogger<ParameterProcessor> _logger;

    public ParameterProcessor(ILogger<ParameterProcessor> logger)
    {
        _logger = logger;
    }

    public List<OpenApiParameter> CreateParameters(List<EndpointArgumentDefinition> args)
    {
        if (args == null)
        {
            _logger.LogError("Args list is null.");
            throw new ArgumentNullException(nameof(args));
        }

        _logger.LogInformation("Creating parameters for args...");

        var parameters = new List<OpenApiParameter>();

        foreach (var arg in args)
        {
            if (arg == null)
            {
                _logger.LogWarning("Found null argument in args.");
                continue;
            }

            if (string.IsNullOrEmpty(arg.Name) || string.IsNullOrEmpty(arg.Type))
            {
                _logger.LogWarning("Found argument with null or empty name or type in args.");
                continue;
            }

            parameters.Add(new OpenApiParameter
            {
                Name = arg.Name,
                Description = arg.Description,
                Required = true,
                Schema = new OpenApiSchema { Type = arg.Type }
            });
        }

        return parameters;
    }
}