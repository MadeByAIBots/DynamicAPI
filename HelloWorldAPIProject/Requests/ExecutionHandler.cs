using HelloWorldAPIProject.Requests;
using HelloWorldAPIProject.Definitions.EndpointDefinitions;
using HelloWorldAPIProject.Definitions.ExecutorDefinitions;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

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

    public async Task<string> ExecuteCommand(EndpointConfiguration endpointConfig)
    {
        var executorConfig = _endpointService.GetExecutorConfiguration(endpointConfig.Executor);
        var executor = _executorFactory.CreateExecutor(endpointConfig.Executor);
        var output = await executor.ExecuteCommand(executorConfig);
        _logger.LogInformation($"Output: {output}");
        return output;
    }
}