using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DynamicApiServer.Definitions.EndpointDefinitions;
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

        public async Task<string> ExecuteCommand(EndpointDefinition endpointConfig, Dictionary<string, string> args)
        {
            var executorConfig = _endpointService.GetExecutorDefinition(endpointConfig);
            var executor = _executorFactory.CreateExecutor(endpointConfig.Executor);
            var output = await executor.ExecuteCommand(executorConfig, args);
            _logger.LogInformation($"Output: {output}");
            return output;
        }
    }
}
