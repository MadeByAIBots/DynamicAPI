using Microsoft.Extensions.Logging;
using DynamicApiServer.Execution;
using DynamicApi.Endpoints.Model;
using DynamicApiServer.Requests.Arguments;


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

        public async Task HandleRequest(EndpointRequestInfo requestInfo, Func<Task> next)
        {
            var endpointConfig = GetEndpointDefinition(requestInfo.Context);

            if (endpointConfig != null)
            {
                LogEndpointInformation(endpointConfig);

                var args = await _argExtractor.ExtractArguments(requestInfo, endpointConfig.Args);

                var output = await _executionHandler.ExecuteCommand(endpointConfig, args);
                await requestInfo.Context.Response.WriteAsync(output);
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
