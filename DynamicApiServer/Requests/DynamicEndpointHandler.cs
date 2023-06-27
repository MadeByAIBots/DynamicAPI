using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using DynamicApiServer.Execution;
using DynamicApiServer.Definitions.EndpointDefinitions;

namespace DynamicApiServer.Requests
{
    public class DynamicEndpointHandler
    {
        private readonly EndpointService _endpointService;
        private readonly ILogger<EndpointService> _logger;
        private readonly ExecutionHandler _executionHandler;
        private readonly EndpointArgumentExtractor _argExtractor;

        public DynamicEndpointHandler(EndpointService endpointService, ILogger<EndpointService> logger, ExecutionHandler executionHandler, EndpointArgumentExtractor argExtractor)
        {
            _endpointService = endpointService;
            _logger = logger;
            _executionHandler = executionHandler;
            _argExtractor = argExtractor;
        }

        public async Task HandleRequest(HttpContext context, Func<Task> next)
        {
            var endpointConfig = GetEndpointDefinition(context);

            if (endpointConfig != null)
            {
                LogEndpointInformation(endpointConfig);

                var args = _argExtractor.ExtractArguments(context, endpointConfig.Args);

                var output = await _executionHandler.ExecuteCommand(endpointConfig, args);
                await context.Response.WriteAsync(output);
            }
            else
            {
                await InvokeNextMiddleware(next);
            }
        }

        private EndpointDefinition GetEndpointDefinition(HttpContext context)
        {
            return _endpointService.GetEndpointDefinition(context.Request.Path);
        }

        private void LogEndpointInformation(EndpointDefinition endpointConfig)
        {
            _logger.LogInformation($"Found matching dynamic endpoint: {endpointConfig.Path}");
            _logger.LogInformation($"Executor: {endpointConfig.Executor}");
        }

        private async Task InvokeNextMiddleware(Func<Task> next)
        {
            await next.Invoke();
        }
    }
}