
namespace DynamicApiServer.Extensions
{
	public static class WebApplicationBuilderExtensions
	{
		public static WebApplication BuildConfiguredApplication(this WebApplicationBuilder builder)
		{
			Console.WriteLine("[INFO] Registering services...");

			var configuration = builder.Configuration;
builder.Services.ConfigureLoggingServices();

			Console.WriteLine("[INFO] Services registered.");

			return builder.Build();
		}
	}
}
