using DynamicApiServer.Definitions.ExecutorDefinitions;
using System.Threading.Tasks;

public interface IEndpointExecutor
{
    Task<string> ExecuteCommand(IExecutorConfiguration executorConfig);
}