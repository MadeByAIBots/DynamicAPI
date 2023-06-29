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

    private readonly WorkingDirectoryResolver _resolver;

    public ExecutorFactory(ApiConfiguration config, WorkingDirectoryResolver resolver, ProcessRunner processRunner, ILoggerFactory loggerFactory)
    {
        this._resolver = resolver;
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
                return new CSharpEndpointExecutor(config, _resolver, loggerFactory);
            case "csharp-script":
                return new CSharpScriptEndpointExecutor(config, _resolver, new CSharpScriptUtilities(_resolver), loggerFactory);
            // Add cases for other executor types as needed...
            default:
                throw new NotSupportedException($"Executor type {executorType} is not supported.");
        }
    }
}