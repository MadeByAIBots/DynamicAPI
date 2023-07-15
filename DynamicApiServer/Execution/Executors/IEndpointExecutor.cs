using DynamicApi.Endpoints.Executors.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using DynamicApi.Endpoints.Model;

public interface IEndpointExecutor
{
    Task<string> ExecuteCommand(EndpointDefinition endpointDefinition, IExecutorDefinition executorDefinision, Dictionary<string, string> args);
}