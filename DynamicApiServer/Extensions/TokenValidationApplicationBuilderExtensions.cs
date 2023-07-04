
namespace DynamicApiServer.Extensions
{
    public static class TokenValidationApplicationBuilderExtensions
    {
        public static void UseTokenValidation(this IApplicationBuilder app)
        {
            var tokenLoader = app.ApplicationServices.GetRequiredService<TokenLoader>();
            var token = tokenLoader.LoadToken();

            app.Use(async (context, next) =>
            {
                if (!IsPubliclyAccessiblePath(context.Request.Path))
                {
                    var authorizationHeader = context.Request.Headers["Authorization"].ToString();
                    if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Unauthorized");
                        return;
                    }

                    var bearerToken = authorizationHeader.Split(' ')[1];
                    if (bearerToken != token)
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Unauthorized");
                        return;
                    }
                }

                await next.Invoke();
            });
        }

        private static bool IsPubliclyAccessiblePath(string path)
        {
            var publiclyAccessiblePaths = GetPubliclyAccessiblePaths();
            return publiclyAccessiblePaths.Contains(path);
        }

        private static List<string> GetPubliclyAccessiblePaths()
        {
            return new List<string> { "/openapi.yaml", "/anotherfile" };
        }
    }
}
