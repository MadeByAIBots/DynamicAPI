using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using DynamicApiServer.Extensions;
using Microsoft.Extensions.Logging;

namespace DynamicApiServer.Tests.Integration
{
	public class TestStartup
	{
		private readonly IConfiguration _configuration;

		public TestStartup(IConfiguration configuration)
		{
			_configuration = configuration;

		}
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddApiServices();
			services.AddRouting();
			services.AddSingleton<TokenLoader>();
			services.ConfigureLoggingServices();
		}

		public void Configure(IApplicationBuilder app)
		{
			app.UseRequestLogging();
			app.UseRouting();
			app.UseInjectableStaticFiles();
			app.UseDynamicEndpoints();
			app.MapFallbackRoute();
		}
	}
}
