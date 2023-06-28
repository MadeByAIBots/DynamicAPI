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
            services.AddLogging(loggingBuilder =>
            {
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