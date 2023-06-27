using DynamicApiServer.Execution.Executors.Bash;

namespace DynamicApiServer.Execution.Executors;

public class ExecutorFactory
{
    public ILoggerFactory loggerFactory { get; set; }

    public ProcessRunner processRunner { get; set; }

    public ExecutorFactory(ProcessRunner processRunner, ILoggerFactory loggerFactory)
    {
        this.processRunner = processRunner;
        this.loggerFactory = loggerFactory;
    }

    public IEndpointExecutor CreateExecutor(string executorType)
    {
        switch (executorType)
        {
            case "bash":
                return new BashEndpointExecutor(loggerFactory, processRunner);
            // Add cases for other executor types as needed...
            default:
                throw new NotSupportedException($"Executor type {executorType} is not supported.");
        }
    }
}