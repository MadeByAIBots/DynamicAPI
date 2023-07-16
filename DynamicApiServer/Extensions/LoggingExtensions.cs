using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DynamicApiServer.Extensions
{
    public static class LoggingExtensions
    {
        public static IServiceCollection ConfigureLoggingServices(this IServiceCollection services)
        {
            // using (var serviceProvider = services.BuildServiceProvider())
            // {
            //     var configuration = serviceProvider.GetRequiredService<IConfiguration>();

                var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var appSettingsPath = environmentName != null ? $"appsettings.{environmentName}.json" : "appsettings.json";

                Console.WriteLine($"Environment: {environmentName ?? "Not set"}");
                Console.WriteLine($"AppSettings Path: {appSettingsPath}");
                services.AddLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
            //        loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
                    loggingBuilder.AddConsole();
                });
            //}

            return services;
        }
    }
}
