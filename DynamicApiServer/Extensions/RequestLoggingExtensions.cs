
namespace DynamicApiServer.Extensions
{
    public static class RequestLoggingExtensions
    {
        public static void UseRequestLogging(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                Console.WriteLine($"Received request: {context.Request.Method} {context.Request.Path}");
                await next.Invoke();
            });
        }
    }
}
