using Microsoft.AspNetCore.Builder;
using System;

namespace HelloWorldAPIProject.Extensions
{
    public static class RequestLoggingExtensions
    {
        public static void UseRequestLogging(this WebApplication app)
        {
            app.Use(async (context, next) =>
            {
                Console.WriteLine($"Received request: {context.Request.Method} {context.Request.Path}");
                await next.Invoke();
            });
        }
    }
}