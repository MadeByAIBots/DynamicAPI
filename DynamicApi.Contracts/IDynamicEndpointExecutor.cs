using System.Collections.Generic;
using System.Threading.Tasks;

namespace DynamicApi.Contracts
{
    public interface IDynamicEndpointExecutor
    {
        Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters);
    }

    public class EndpointExecutionResult
    {
        public string Body { get; set; }
    }

    public class DynamicExecutionParameters
    {
        public Dictionary<string, string> Parameters { get; set; }

        public DynamicExecutionParameters(Dictionary<string, string> parameters)
        {
            Parameters = parameters;
        }
    }
}