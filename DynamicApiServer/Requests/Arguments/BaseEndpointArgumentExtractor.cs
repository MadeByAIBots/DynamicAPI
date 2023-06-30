using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace DynamicApiServer.Requests.Arguments
{
    public abstract class BaseEndpointArgumentExtractor
    {
        protected readonly ILogger _logger;

        public BaseEndpointArgumentExtractor(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<Dictionary<string, string>> ExtractArguments(EndpointRequestInfo requestInfo, List<EndpointArgumentDefinition> argumentDefinitions)
        {
            var arguments = new Dictionary<string, string>();

            foreach (var argumentDefinition in argumentDefinitions)
            {
                try
                {
                    _logger.LogInformation($"Starting extraction of argument: {argumentDefinition.Name}");
                    var argumentValue = await ExtractSingleArgument(requestInfo, argumentDefinition);
                    if (argumentValue == null)
                    {
                        _logger.LogWarning($"Missing argument: {argumentDefinition.Name}");
                    }
                    else
                    {
                        arguments[argumentDefinition.Name] = argumentValue;
                        _logger.LogInformation($"Successfully extracted argument: {argumentDefinition.Name}");
                    }
                }
                catch (Exception ex)
                {
                    // Log the error and rethrow the exception
                    _logger.LogError(ex, $"Error extracting argument: {argumentDefinition.Name}");
                    throw;
                }
            }

            return arguments;
        }

        protected abstract Task<string> ExtractSingleArgument(EndpointRequestInfo requestInfo, EndpointArgumentDefinition argumentDefinition);
    }
}