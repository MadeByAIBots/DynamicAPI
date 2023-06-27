using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using DynamicApiServer.Requests;

namespace DynamicApiServer.Extensions
{
    public static class DynamicEndpointsExtensions
    {
        public static void UseDynamicEndpoints(this WebApplication app)
        {
            app.Use(async (context, next) =>
            {
                var handler = app.Services.GetRequiredService<DynamicEndpointHandler>();
                await handler.HandleRequest(context, next);
            });
        }
    }
}