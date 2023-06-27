using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using System;

namespace DynamicApiServer.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplication BuildConfiguredApplication(this WebApplicationBuilder builder)
        {
            Console.WriteLine("[INFO] Registering services...");

            // Register services
            builder.Services.AddLogging();

            Console.WriteLine("[INFO] Services registered.");

            return builder.Build();
        }
    }
}