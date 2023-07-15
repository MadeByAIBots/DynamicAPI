using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using DynamicApi.Endpoints.Model;
using Microsoft.Extensions.Logging;

public class ParameterProcessor
{
    private readonly ILogger<ParameterProcessor> _logger;

    public ParameterProcessor(ILogger<ParameterProcessor> logger)
    {
        _logger = logger;
    }

    public (List<OpenApiParameter> Parameters, OpenApiRequestBody RequestBody) CreateParameters(List<EndpointArgumentDefinition> args)
    {
        if (args == null)
        {
            _logger.LogError("Args list is null.");
            throw new ArgumentNullException(nameof(args));
        }

        _logger.LogInformation("Creating parameters for args...");

        var parameters = new List<OpenApiParameter>();
        var requestBodyProperties = new Dictionary<string, OpenApiSchema>();
        var requiredProperties = new HashSet<string>();

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

            if (arg.Source.ToLower() == "body")
            {
                requestBodyProperties.Add(arg.Name, new OpenApiSchema { Type = MapTypeToOpenApiType(arg.Type), Description = arg.Description });
                if (arg.Required)
                {
                    requiredProperties.Add(arg.Name);
                }
            }
            else
            {
                parameters.Add(new OpenApiParameter
                {
                    Name = arg.Name,
                    Description = arg.Description,
                    Required = true,
                    In = MapSourceToIn(arg.Source),
                    Schema = new OpenApiSchema { Type = MapTypeToOpenApiType(arg.Type) }
                });
            }
        }

        var requestBody = requestBodyProperties.Count > 0 ? new OpenApiRequestBody
        {
            Description = "request body",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/json"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = requestBodyProperties,
                        Required = requiredProperties
                    }
                }
            }
        } : null;

        return (parameters, requestBody);
    }

    private ParameterLocation MapSourceToIn(string source)
    {
        return source.ToLower() switch
        {
            "query" => ParameterLocation.Query,
            "header" => ParameterLocation.Header,
            "path" => ParameterLocation.Path,
            "cookie" => ParameterLocation.Cookie,
            _ => throw new ArgumentException($"Invalid source: {source}")
        };
    }

    private string MapTypeToOpenApiType(string type)
    {
        var validTypes = new HashSet<string> { "string", "number", "boolean", "integer", "array", "object" };

        if (validTypes.Contains(type.ToLower()))
        {
            return type.ToLower();
        }
        else
        {
            return "string";
        }
    }
}
