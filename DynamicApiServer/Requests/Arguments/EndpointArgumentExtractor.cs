using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace DynamicApiServer.Requests.Arguments
{
    public class EndpointArgumentExtractor
    {
        private readonly Dictionary<string, BaseEndpointArgumentExtractor> _extractors;
        private readonly ILogger _logger;

        public EndpointArgumentExtractor(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<EndpointArgumentExtractor>();
            _extractors = new Dictionary<string, BaseEndpointArgumentExtractor>
            {
                { "query", new QueryEndpointArgumentExtractor(loggerFactory) },
                { "body", new BodyEndpointArgumentExtractor(loggerFactory) },
            };
        }

        public async Task<Dictionary<string, string>> ExtractArguments(HttpContext httpContext, List<EndpointArgumentDefinition> argumentDefinitions)
        {
            var arguments = new Dictionary<string, string>();

            if (argumentDefinitions == null || argumentDefinitions.Count == 0)
                return new Dictionary<string, string>();

            foreach (var argumentDefinition in argumentDefinitions)
            {
                if (_extractors.TryGetValue(argumentDefinition.Source, out var extractor))
                {
                    try
                    {
                        var extractedArguments = await extractor.ExtractArguments(httpContext, argumentDefinitions);
                        foreach (var arg in extractedArguments)
                        {
                            arguments[arg.Key] = arg.Value;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error and rethrow the exception
                        _logger.LogError(ex, $"Error extracting arguments for source: {argumentDefinition.Source}");
                        throw;
                    }
                }
                else
                {
                    // Log an error message and throw an exception
                    var errorMessage = $"Unsupported argument source: {argumentDefinition.Source}";
                    _logger.LogError(errorMessage);
                    throw new NotSupportedException(errorMessage);
                }
            }

            return arguments;
        }
    }
}