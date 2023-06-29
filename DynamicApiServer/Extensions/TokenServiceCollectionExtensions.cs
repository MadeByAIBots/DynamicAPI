namespace DynamicApiServer.Extensions
{
    public static class TokenServiceCollectionExtensions
    {
        public static void AddToken(this IServiceCollection services, string tokenFilePath)
        {
            // TODO: Check if needed
            // var tokenLoader = new TokenLoader();
            // var token = tokenLoader.LoadToken(tokenFilePath);

            // services.AddSingleton(token);
        }
    }
}