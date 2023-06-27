using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;

using DynamicApiServer.Tests.Integration;

public class IntegrationTestContext : IDisposable
{
    public TestServer Server { get; }
    public HttpClient Client { get; }

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

    public void Dispose()
    {
        Client.Dispose();
        Server.Dispose();
    }
}