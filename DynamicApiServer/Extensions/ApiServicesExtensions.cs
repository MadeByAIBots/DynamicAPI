using Microsoft.Extensions.DependencyInjection;
using DynamicApiServer.Requests;
using DynamicApiServer.Execution;
using DynamicApiConfiguration;
using DynamicApiServer.Execution.Executors.Bash;
using DynamicApiServer.Execution.Executors;
using DynamicApiServer.Requests.Arguments;

namespace DynamicApiServer.Extensions
{
    public static class ApiServicesExtensions
    {
        public static void AddApiServices(this IServiceCollection services)
        {
            var configLoader = new ConfigurationLoader();
            var config = configLoader.LoadConfiguration("/root/workspace/DynamicAPI/config.json");

            services.AddSingleton(config);

            services.AddSingleton<DynamicEndpointHandler>();
            services.AddSingleton<EndpointArgumentExtractor>();

            services.AddSingleton<EndpointLoader>();
            services.AddSingleton<EndpointService>();

            services.AddSingleton<BashEndpointExecutor>();
            services.AddSingleton<ExecutionHandler>();
            services.AddSingleton<ExecutorFactory>();
            services.AddSingleton<ExecutorDefinitionLoader>();

            services.AddSingleton<ProcessRunner>();
        }
    }
}