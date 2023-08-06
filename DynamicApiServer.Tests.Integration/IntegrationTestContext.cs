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
    private readonly WorkingDirectoryResolver _workingDirectoryResolver;
    public TestServer Server { get; }
    public HttpClient Client { get; }
    private TestEndpointManager _endpointManager;
    public WebHostBuilder Builder { get; set; }

    public IntegrationTestContext()
    {
        Builder = new WebHostBuilder();
        Builder.UseStartup<TestStartup>();
        
        Server = new TestServer(Builder);
        Client = Server.CreateClient();

        _workingDirectoryResolver = Server.Services.GetRequiredService<WorkingDirectoryResolver>();
    }

    private void Configure(WebApplication app)
    {
        // Configure the application...
    }

    public TestEndpointManager Endpoint()
    {
        // TODO: Clean up this function

        var resolver = _workingDirectoryResolver;
        var loggerFactory = Server.Services.GetRequiredService<ILoggerFactory>();
        // TODO: Remove hardcoded path
        _endpointManager = new TestEndpointManager(resolver.WorkingDirectory() + "/endpoints", loggerFactory);
        return _endpointManager;
    }

    public void UseToken()
    {
        var tokenLoader = Server.Services.GetRequiredService<TokenLoader>();
        var token = tokenLoader.LoadToken();

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
