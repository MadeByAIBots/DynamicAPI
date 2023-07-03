using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using DynamicApiServer.Extensions;
using Microsoft.Extensions.Logging;

namespace DynamicApiServer.Tests.Integration
{
    public class TestStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApiServices();
            services.AddRouting();
services.AddSingleton<TokenLoader>();
            services.AddLogging(loggingBuilder =>
            {
loggingBuilder.ClearProviders();
var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
var logLevelString = configuration.GetSection("Logging:LogLevel:Default").Value;
if (!Enum.TryParse<LogLevel>(logLevelString, out var logLevel))
{
    logLevel = LogLevel.Information;
}
Console.WriteLine($"Setting log level to {logLevel}");
logLevel = LogLevel.Error;
loggingBuilder.AddFilter(level => level >= logLevel);
loggingBuilder.AddConsole();



            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRequestLogging();
            app.UseRouting();
            app.UseDynamicEndpoints();
            app.MapFallbackRoute();
        }
    }
}
