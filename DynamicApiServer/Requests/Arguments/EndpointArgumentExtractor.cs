using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamicApiServer.Definitions.EndpointDefinitions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DynamicApiServer.Requests.Arguments
{
    public class EndpointArgumentExtractor
    {
        private readonly ILogger _logger;
        private readonly Dictionary<string, BaseEndpointArgumentExtractor> _extractors;

        public EndpointArgumentExtractor(ILoggerFactory loggerFactory, IEnumerable<BaseEndpointArgumentExtractor> extractors)
        {
            _logger = loggerFactory.CreateLogger<EndpointArgumentExtractor>();
            _extractors = new Dictionary<string, BaseEndpointArgumentExtractor>
            {
                                { "query", new QueryEndpointArgumentExtractor(loggerFactory) },
                { "body", new BodyEndpointArgumentExtractor(loggerFactory) },
            };
        }

        public async Task<Dictionary<string, string>> ExtractArguments(EndpointRequestInfo requestInfo, List<EndpointArgumentDefinition> argumentDefinitions)
        {
            var arguments = new Dictionary<string, string>();

            if (argumentDefinitions == null || argumentDefinitions.Count == 0)
            {
                _logger.LogInformation("No argument definitions provided.");
                return arguments;
            }

            foreach (var source in _extractors.Keys)
            {
                var matchingDefinitions = argumentDefinitions.Where(def => def.Source == source).ToList();
                if (matchingDefinitions.Count > 0 && _extractors.TryGetValue(source, out var extractor))
                {
                    _logger.LogInformation($"Extracting arguments for source: {source}");
                    try
                    {
                        var extractedArguments = await extractor.ExtractArguments(requestInfo, matchingDefinitions);
                        foreach (var arg in extractedArguments)
                        {
                            arguments[arg.Key] = arg.Value;
                        }
                        _logger.LogInformation($"Successfully extracted arguments for source: {source}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error extracting arguments for source: {source}");
                        throw;
                    }
                }
                else
                {
                    _logger.LogWarning($"No matching argument definitions for source: {source}");
                }
            }

            return arguments;
        }
    }
}