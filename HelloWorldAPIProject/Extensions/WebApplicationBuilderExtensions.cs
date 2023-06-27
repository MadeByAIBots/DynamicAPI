using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using System;

namespace HelloWorldAPIProject.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplication BuildConfiguredApplication(this WebApplicationBuilder builder)
        {
            Console.WriteLine("[INFO] Registering services...");

            // Register services
            builder.Services.AddLogging();
            builder.Services.AddTransient<RequestProcessor>();
            //builder.Services.AddTransient<EndpointRouteConfigurator>();

            Console.WriteLine("[INFO] Services registered.");

            return builder.Build();
        }
    }
}