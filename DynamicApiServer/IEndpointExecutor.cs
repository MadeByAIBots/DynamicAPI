using DynamicApiServer.Definitions.ExecutorDefinitions;
using System.Threading.Tasks;

namespace DynamicApiServer
{
    public interface IEndpointExecutor
    {
        Task<string> ExecuteCommand(BashExecutorConfiguration configuration);
    }
}