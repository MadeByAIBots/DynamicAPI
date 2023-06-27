using Microsoft.Extensions.DependencyInjection;
using DynamicApiServer.Requests;

namespace DynamicApiServer.Extensions
{
    public static class ApiServicesExtensions
    {
        public static void AddApiServices(this IServiceCollection services)
        {
            services.AddSingleton<EndpointService>(sp => new EndpointService("/root/workspace/DynamicAPI/config/endpoints"));
            services.AddSingleton<RequestProcessor>();
            services.AddSingleton<BashEndpointExecutor>();
            services.AddSingleton<DynamicEndpointHandler>();
            services.AddSingleton<ExecutionHandler>();
            services.AddSingleton<ExecutorFactory>();
        }
    }
}