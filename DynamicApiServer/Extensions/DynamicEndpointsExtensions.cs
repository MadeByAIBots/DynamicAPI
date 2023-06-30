using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using DynamicApiServer.Requests;

namespace DynamicApiServer.Extensions
{
    public static class DynamicEndpointsExtensions
    {
        public static void UseDynamicEndpoints(this IApplicationBuilder app)
        {
            app.UseRouting();
            
            app.Use(async (context, next) =>
            {
                if (!IsAuthorized(context, app))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }

                var handler = app.ApplicationServices.GetRequiredService<DynamicEndpointHandler>();
                await handler.HandleRequest(new EndpointRequestInfo(context), next);
            });
        }

        private static bool IsAuthorized(HttpContext context, IApplicationBuilder app)
        {
            // TODO: Move this to a dedicated class
            
            var tokenLoader = app.ApplicationServices.GetRequiredService<TokenLoader>();
            var token = tokenLoader.LoadToken();

            var authorizationHeader = context.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return false;
            }

            var bearerToken = authorizationHeader.Split(' ')[1];
            return bearerToken == token;
        }

    }
}