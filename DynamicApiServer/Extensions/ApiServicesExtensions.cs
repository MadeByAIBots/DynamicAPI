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
            // TODO: Clean up this function
            var resolver = new WorkingDirectoryResolver();

            var configLoader = new ConfigurationLoader();
            var config = configLoader.LoadConfiguration(resolver.WorkingDirectory() + "/config.json");

            services.AddSingleton(config);

            services.AddSingleton<TokenLoader>();

            services.AddSingleton<WorkingDirectoryResolver>();

            services.AddToken(resolver.WorkingDirectory() + "/" + config.TokenFilePath);

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