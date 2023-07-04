
namespace DynamicApiServer.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplication BuildConfiguredApplication(this WebApplicationBuilder builder)
        {
            Console.WriteLine("[INFO] Registering services...");

            var configuration = builder.Configuration;
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
            });

            Console.WriteLine("[INFO] Services registered.");

            return builder.Build();
        }
    }
}
