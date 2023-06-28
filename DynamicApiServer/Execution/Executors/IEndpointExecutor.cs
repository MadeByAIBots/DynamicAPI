using DynamicApiServer.Definitions.ExecutorDefinitions;
using System.Collections.Generic;
using System.Threading.Tasks;
using DynamicApiServer.Definitions.EndpointDefinitions;

public interface IEndpointExecutor
{
    Task<string> ExecuteCommand(EndpointDefinition endpointDefinition, IExecutorDefinition executorDefinision, Dictionary<string, string> args);
}