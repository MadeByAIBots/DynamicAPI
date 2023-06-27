using Microsoft.Extensions.DependencyInjection;
using HelloWorldAPIProject.Requests;

namespace HelloWorldAPIProject.Extensions
{
    public static class ApiServicesExtensions
    {
        public static void AddApiServices(this IServiceCollection services)
        {
            services.AddSingleton<EndpointService>(sp => new EndpointService("/root/workspace/DynamicAPI/config/endpoints"));
            services.AddSingleton<RequestProcessor>();
            services.AddSingleton<EndpointExecutor>();
            services.AddSingleton<DynamicEndpointHandler>();
        }
    }
}