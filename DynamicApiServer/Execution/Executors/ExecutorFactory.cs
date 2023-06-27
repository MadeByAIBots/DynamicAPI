public class ExecutorFactory
{
    public ILoggerFactory loggerFactory { get;set;}

    public ExecutorFactory(ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
    }

    public IEndpointExecutor CreateExecutor(string executorType)
    {
        switch (executorType)
        {
            case "bash":
                return new BashEndpointExecutor(loggerFactory);
            // Add cases for other executor types as needed...
            default:
                throw new NotSupportedException($"Executor type {executorType} is not supported.");
        }
    }
}