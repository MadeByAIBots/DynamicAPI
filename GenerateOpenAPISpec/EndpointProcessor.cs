using System;
using Microsoft.OpenApi.Models;
using DynamicApiServer.Definitions.EndpointDefinitions;
using Microsoft.Extensions.Logging;

public class EndpointProcessor
{
    private readonly ILogger<EndpointProcessor> _logger;
    private readonly OperationProcessor _operationProcessor;

    public EndpointProcessor(ILogger<EndpointProcessor> logger, OperationProcessor operationProcessor)
    {
        _logger = logger;
        _operationProcessor = operationProcessor;
    }

    public OpenApiPathItem ProcessEndpoint(EndpointDefinition endpointDefinition)
    {
        if (endpointDefinition == null)
        {
            _logger.LogError("Endpoint definition is null.");
            throw new ArgumentNullException(nameof(endpointDefinition));
        }

        _logger.LogInformation($"Processing endpoint definition: Method = {endpointDefinition.Method}, Path = {endpointDefinition.Path}");

        var pathItem = new OpenApiPathItem();
        var operation = _operationProcessor.CreateOperation(endpointDefinition);

        switch (endpointDefinition.Method?.ToLower())
        {
            case "get":
                pathItem.Operations.Add(OperationType.Get, operation);
                break;
            case "post":
                pathItem.Operations.Add(OperationType.Post, operation);
                break;
            // Add more cases here if you have other methods
            default:
                _logger.LogWarning($"Unsupported method '{endpointDefinition.Method}' in endpoint definition.");
                break;
        }

        return pathItem;
    }
}