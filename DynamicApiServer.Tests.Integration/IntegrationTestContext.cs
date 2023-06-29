using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace DynamicApiServer.Tests.Integration;

public class IntegrationTestContext : IDisposable
{
    public TestServer Server { get; }
    public HttpClient Client { get; }
    private TestEndpointManager _endpointManager;

    public IntegrationTestContext()
    {
        var builder = WebApplication.CreateBuilder(new string[0]);
        var app = builder.Build();
        Configure(app);
        Server = new TestServer(new WebHostBuilder().UseStartup<TestStartup>().UseContentRoot(app.Environment.ContentRootPath));
        Client = Server.CreateClient();

    }

    private void Configure(WebApplication app)
    {
        // Configure the application...
    }

    public TestEndpointManager Endpoint()
    {
        var loggerFactory = Server.Services.GetRequiredService<ILoggerFactory>();
        // TODO: Remove hardcoded path
        _endpointManager = new TestEndpointManager("../../../config/endpoints", loggerFactory);
        return _endpointManager;
    }

    public void UseToken()
    {
        var config = Server.Services.GetRequiredService<ApiConfiguration>();
        var tokenLoader = Server.Services.GetRequiredService<TokenLoader>();
        var token = tokenLoader.LoadToken(config.TokenFilePath);

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }


    public void Dispose()
    {
        Client.Dispose();
        Server.Dispose();

        if (_endpointManager != null)
            _endpointManager.Dispose();
    }
}