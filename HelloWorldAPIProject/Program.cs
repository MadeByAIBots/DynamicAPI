using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<EndpointService>(new EndpointService("/root/workspace/DynamicAPI/config/endpoints/"));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}



app.UseRouting();
var endpointService = app.Services.GetRequiredService<EndpointService>();
var endpointConfigurations = endpointService.LoadConfigurations();

var endpoints = app.UseEndpoints(endpoints =>
{
    foreach (var config in endpointConfigurations)
    {
        var executorConfig = endpointService.GetExecutorConfiguration(config.Executor);
        var executor = new EndpointExecutor(executorConfig);
        executor.Execute(endpoints, config);
    }
});

app.Run();