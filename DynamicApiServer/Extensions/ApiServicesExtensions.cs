
namespace DynamicApiServer.Extensions
{
    public static class ApiServicesExtensions
    {
        public static void AddApiServices(this IServiceCollection services)
        {
services.AddSingleton<ConfigurationLoader>();
services.AddSingleton(provider =>
            {
                var configLoader = provider.GetRequiredService<ConfigurationLoader>();
                return configLoader.LoadConfiguration();
            });
            services.AddSingleton<WorkingDirectoryResolver>();

            var provider = services.BuildServiceProvider();
            var config = provider.GetRequiredService<ApiConfiguration>();
            var resolver = provider.GetRequiredService<WorkingDirectoryResolver>();
            var tokenFilePath = resolver.WorkingDirectory() + "/" + config.TokenFilePath;

services.AddSingleton<DynamicApiServer.Requests.DynamicEndpointHandler>();
            services.AddSingleton<DynamicApiServer.Requests.Arguments.EndpointArgumentExtractor>();
            services.AddSingleton<EndpointLoader>();
            services.AddSingleton<EndpointService>();
            services.AddSingleton<DynamicApiServer.Execution.Executors.Bash.BashEndpointExecutor>();
            services.AddSingleton<DynamicApiServer.Execution.ExecutionHandler>();
            services.AddSingleton<DynamicApiServer.Execution.Executors.ExecutorFactory>();
            services.AddSingleton<ExecutorDefinitionLoader>();
            services.AddSingleton<ProcessRunner>();
            services.AddToken(tokenFilePath);
        }
    }
}
