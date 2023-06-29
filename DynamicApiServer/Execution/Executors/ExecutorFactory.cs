using DynamicApiServer.Execution.Executors.Bash;
using DynamicApiServer.Execution.Executors.CSharp;
using DynamicApiServer.Execution.Executors.CSharpScript;
using DynamicApiConfiguration;

namespace DynamicApiServer.Execution.Executors;

public class ExecutorFactory
{
    public ILoggerFactory loggerFactory { get; set; }

    public ProcessRunner processRunner { get; set; }

    public ApiConfiguration config { get; set; }

    public ExecutorFactory(ApiConfiguration config, ProcessRunner processRunner, ILoggerFactory loggerFactory)
    {
        this.config = config;
        this.processRunner = processRunner;
        this.loggerFactory = loggerFactory;
    }

    public IEndpointExecutor CreateExecutor(string executorType)
    {
        switch (executorType)
        {
            case "bash":
                return new BashEndpointExecutor(loggerFactory, processRunner);
            case "csharp":
                return new CSharpEndpointExecutor(config, loggerFactory);
            case "csharp-script":
                return new CSharpScriptEndpointExecutor(config, loggerFactory);
            // Add cases for other executor types as needed...
            default:
                throw new NotSupportedException($"Executor type {executorType} is not supported.");
        }
    }
}