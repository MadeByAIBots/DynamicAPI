using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicApiServer.Extensions
{
    public static class TokenValidationApplicationBuilderExtensions
    {
        public static void UseTokenValidation(this IApplicationBuilder app)
        {
            var token = app.ApplicationServices.GetRequiredService<string>();

            app.Use(async (context, next) =>
            {
                var bearerToken = context.Request.Headers["Authorization"].ToString().Split(' ')[1];
                if (bearerToken != token)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }

                await next.Invoke();
            });
        }
    }
}