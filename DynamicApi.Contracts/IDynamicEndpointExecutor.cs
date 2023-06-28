using System.Collections.Generic;
using System.Threading.Tasks;

namespace DynamicApi.Contracts
{
    public interface IDynamicEndpointExecutor
    {
        Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters);
    }
}