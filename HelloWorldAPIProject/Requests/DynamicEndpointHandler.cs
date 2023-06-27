using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using HelloWorldAPIProject.Requests;
using HelloWorldAPIProject.Definitions.EndpointDefinitions;
using HelloWorldAPIProject.Definitions.ExecutorDefinitions;

namespace HelloWorldAPIProject.Requests
{
    public class DynamicEndpointHandler
    {
        private readonly EndpointService _endpointService;
        private readonly ILogger<EndpointService> _logger;
        private readonly EndpointExecutor _executor;

        public DynamicEndpointHandler(EndpointService endpointService, ILogger<EndpointService> logger, EndpointExecutor executor)
        {
            _endpointService = endpointService;
            _logger = logger;
            _executor = executor;
        }

        public async Task HandleRequest(HttpContext context, Func<Task> next)
        {
            var endpointConfig = GetEndpointConfiguration(context);

            if (endpointConfig != null)
            {
                LogEndpointInformation(endpointConfig);

                if (endpointConfig.Executor == "bash")
                {
                    var executorConfig = _endpointService.GetExecutorConfiguration(endpointConfig.Executor);
                    await ExecuteCommandAndWriteResponse(context, executorConfig);
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

        private async Task ExecuteCommandAndWriteResponse(HttpContext context, BashExecutorConfiguration executorConfig)
        {
            var output = await _executor.ExecuteCommand(executorConfig);
            _logger.LogInformation($"Output: {output}");
            await context.Response.WriteAsync(output);
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