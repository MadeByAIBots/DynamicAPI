using HelloWorldAPIProject.Definitions.ExecutorDefinitions;
using System.Threading.Tasks;

namespace HelloWorldAPIProject
{
    public interface IEndpointExecutor
    {
        Task<string> ExecuteCommand(BashExecutorConfiguration configuration);
    }
}