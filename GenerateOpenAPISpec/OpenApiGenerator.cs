using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using DynamicApiServer.Definitions.EndpointDefinitions;
using Microsoft.OpenApi.Models;

public class OpenApiGenerator
{
    private readonly ILogger<OpenApiGenerator> _logger;
    private readonly EndpointProcessor _endpointProcessor;
    private readonly DocumentGenerator _documentGenerator;

    public OpenApiGenerator(ILogger<OpenApiGenerator> logger, EndpointProcessor endpointProcessor, DocumentGenerator documentGenerator)
    {
        _logger = logger;
        _endpointProcessor = endpointProcessor;
        _documentGenerator = documentGenerator;
    }

    public string GenerateOpenApiSpec(List<EndpointDefinition> endpointDefinitions)
    {
        if (endpointDefinitions == null)
        {
            _logger.LogError("Endpoint definitions list is null.");
            throw new ArgumentNullException(nameof(endpointDefinitions));
        }

        _logger.LogInformation("Generating OpenAPI specification...");

        var paths = new Dictionary<string, OpenApiPathItem>();

        foreach (var endpointDefinition in endpointDefinitions)
        {
            var pathItem = _endpointProcessor.ProcessEndpoint(endpointDefinition);
            paths.Add(endpointDefinition.Path, pathItem);
        }

        var openApiSpec = _documentGenerator.GenerateDocument(paths);

        _logger.LogInformation("Finished generating OpenAPI specification.");

        return openApiSpec;
    }
}