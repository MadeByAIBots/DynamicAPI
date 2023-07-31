using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DynamicApi.Endpoints.Model;
using DynamicApiServer.Execution.Executors;

namespace DynamicApiServer.Execution
{
    public class ExecutionHandler
    {
        private readonly EndpointService _endpointService;
        private readonly ILogger<EndpointService> _logger;
        private readonly ExecutorFactory _executorFactory;

        public ExecutionHandler(EndpointService endpointService, ILogger<EndpointService> logger, ExecutorFactory executorFactory)
        {
            _endpointService = endpointService;
            _logger = logger;
            _executorFactory = executorFactory;
        }

        public async Task<string> ExecuteCommand(EndpointDefinition endpointDefinition, Dictionary<string, string> args)
        {
            try
            {

                var executorDefinition = _endpointService.GetExecutorDefinition(endpointDefinition);
                var executor = _executorFactory.CreateExecutor(endpointDefinition.Executor);
                var output = await executor.ExecuteCommand(endpointDefinition, executorDefinition, args);
                _logger.LogInformation($"Output: {output}");

                if (String.IsNullOrEmpty(output))
                    output = "[Endpoint returned empty response]";
                
                return output;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return e.ToString();
            }
        }
    }
}
