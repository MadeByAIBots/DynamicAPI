using DynamicApiServer.Definitions.ExecutorDefinitions;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IEndpointExecutor
{
    Task<string> ExecuteCommand(IExecutorDefinition executorConfig, Dictionary<string, string> args);
}