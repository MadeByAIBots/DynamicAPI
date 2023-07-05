using DynamicApiServer.Execution.Executors.Bash;
using DynamicApiServer.Execution.Executors.CSharp;
using DynamicApiServer.Execution.Executors.CSharpScript;
using DynamicApiConfiguration;

namespace DynamicApiServer.Execution.Executors;

public class ExecutorFactory
{
	private readonly IServiceProvider _serviceProvider;

	public ExecutorFactory(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public IEndpointExecutor CreateExecutor(string executorType)
	{
		switch (executorType)
		{
			case "bash":
				return _serviceProvider.GetRequiredService<BashEndpointExecutor>();
			case "csharp":
				return _serviceProvider.GetRequiredService<CSharpEndpointExecutor>();
			case "csharp-script":
				return _serviceProvider.GetRequiredService<CSharpScriptEndpointExecutor>();
			// Add cases for other executor types as needed...
			default:
				throw new NotSupportedException($"Executor type {executorType} is not supported.");
		}
	}
}
