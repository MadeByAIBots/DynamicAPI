using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using DynamicApiServer.Requests;
using DynamicApiServer.Definitions.EndpointDefinitions;
using DynamicApiServer.Definitions.ExecutorDefinitions;

namespace DynamicApiServer.Requests
{
    public class DynamicEndpointHandler
    {
        private readonly EndpointService _endpointService;
        private readonly ILogger<EndpointService> _logger;
        private readonly ExecutionHandler _executionHandler;

        public DynamicEndpointHandler(EndpointService endpointService, ILogger<EndpointService> logger, ExecutionHandler executionHandler)
        {
            _endpointService = endpointService;
            _logger = logger;
            _executionHandler = executionHandler;
        }

        public async Task HandleRequest(HttpContext context, Func<Task> next)
        {
            var endpointConfig = GetEndpointConfiguration(context);

            if (endpointConfig != null)
            {
                LogEndpointInformation(endpointConfig);

                if (endpointConfig.Executor == "bash")
                {
                    var output = await _executionHandler.ExecuteCommand(endpointConfig);
                    await context.Response.WriteAsync(output);
                }
                else
                {
                    HandleUnsupportedExecutor(endpointConfig);
                }
            }
            else
            {
                await InvokeNextMiddleware(next);
            }
        }

        private EndpointConfiguration GetEndpointConfiguration(HttpContext context)
        {
            return _endpointService.GetEndpointConfiguration(context.Request.Path);
        }

        private void LogEndpointInformation(EndpointConfiguration endpointConfig)
        {
            _logger.LogInformation($"Found matching dynamic endpoint: {endpointConfig.Path}");
            _logger.LogInformation($"Executor: {endpointConfig.Executor}");
        }

        private void HandleUnsupportedExecutor(EndpointConfiguration endpointConfig)
        {
            _logger.LogInformation($"Executor type {endpointConfig.Executor} not supported.");
        }

        private async Task InvokeNextMiddleware(Func<Task> next)
        {
            await next.Invoke();
        }
    }
}