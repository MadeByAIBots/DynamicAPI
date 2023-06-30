using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using DynamicApiServer.Definitions.EndpointDefinitions;
using Microsoft.Extensions.Logging;

public class OperationProcessor
{
    private readonly ILogger<OperationProcessor> _logger;
    private readonly ParameterProcessor _parameterProcessor;

    public OperationProcessor(ILogger<OperationProcessor> logger, ParameterProcessor parameterProcessor)
    {
        _logger = logger;
        _parameterProcessor = parameterProcessor;
    }

    public OpenApiOperation CreateOperation(EndpointDefinition endpointDefinition)
    {
        if (endpointDefinition == null)
        {
            _logger.LogError("Endpoint definition is null.");
            throw new ArgumentNullException(nameof(endpointDefinition));
        }

        var operation = new OpenApiOperation();

        // Set the operationId property of the operation.
        operation.OperationId = ConvertEndpointNameToOperationId(endpointDefinition.Path);

        if (endpointDefinition.Responses != null)
        {
            operation.Responses = new OpenApiResponses();
            foreach (var responseDefinition in endpointDefinition.Responses)
            {
                var response = new OpenApiResponse
                {
                    Description = responseDefinition.Description
                    // Add more properties as needed to represent the structure of a response
                };
                operation.Responses.Add(responseDefinition.StatusCode.ToString(), response);
            }
        }

        _logger.LogInformation($"Creating operation for endpoint definition: Method = {endpointDefinition.Method}, Path = {endpointDefinition.Path}");

        if (endpointDefinition.Args != null)
        {
            var (parameters, requestBody) = _parameterProcessor.CreateParameters(endpointDefinition.Args);
            operation.Parameters = parameters;
            operation.RequestBody = requestBody;
        }

        return operation;
    }

    private string ConvertEndpointNameToOperationId(string endpointName)
    {
        // Remove the leading slash from the endpoint name.
        if (endpointName.StartsWith("/"))
        {
            endpointName = endpointName.Substring(1);
        }

        var words = endpointName.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < words.Length; i++)
        {
            words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
        }
        return string.Join("", words);
    }
}