using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using DynamicApiServer.Definitions.EndpointDefinitions;

namespace DynamicApiServer.Requests
{
    public class EndpointArgumentExtractor
    {
        private readonly ILogger<EndpointArgumentExtractor> _logger;

        public EndpointArgumentExtractor(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<EndpointArgumentExtractor>();
        }

        public Dictionary<string, string> ExtractArguments(HttpContext context, List<EndpointArgumentDefinition> argDefinitions)
        {
            var args = new Dictionary<string, string>();

            if (argDefinitions != null)
            {
                foreach (var arg in argDefinitions)
                {
                    ExtractArgument(context, arg, args);
                }
            }

            return args;
        }

        private void ExtractArgument(HttpContext context, EndpointArgumentDefinition arg, Dictionary<string, string> args)
        {
            try
            {
                string argValue = arg.Source switch
                {
                    "query" => ExtractFromQuery(context, arg),
                    _ => throw new NotSupportedException($"Unsupported argument source: {arg.Source}")
                };

                if (argValue == null)
                {
                    _logger.LogWarning($"Missing argument: {arg.Name}");
                }
                else
                {
                    args[arg.Name] = argValue;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error extracting argument: {arg.Name}");
            }
        }

        private string ExtractFromQuery(HttpContext context, EndpointArgumentDefinition arg)
        {
            return context.Request.Query[arg.Name];
        }

        // Add more methods here for other sources
    }
}
