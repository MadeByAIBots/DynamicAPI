using Microsoft.AspNetCore.Builder;

namespace DynamicApiServer.Extensions
{
    public static class EndpointMappingExtensions
    {
        public static void MapFallbackRoute(this WebApplication app)
        {
            app.MapFallback(context =>
            {
                context.Response.StatusCode = 404;
                return context.Response.WriteAsync("Endpoint not found");
            });
        }
    }
}