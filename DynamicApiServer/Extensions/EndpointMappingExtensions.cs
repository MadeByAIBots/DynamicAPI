using Microsoft.AspNetCore.Builder;

namespace DynamicApiServer.Extensions
{
    public static class EndpointMappingExtensions
    {
        public static void MapFallbackRoute(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapFallback(context =>
                {
                    context.Response.StatusCode = 404;
                    return context.Response.WriteAsync("Endpoint not found");
                });
            });
        }
    }
}