using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

Console.WriteLine("Loading endpoint configurations...");

// Load the endpoint configurations
var endpointLoader = new EndpointLoader("/root/workspace/DynamicAPI/config");
var endpointConfigurations = endpointLoader.LoadConfigurations();

Console.WriteLine("Creating endpoints...");

// Create the endpoints
var endpointExecutor = new EndpointExecutor();
endpointExecutor.CreateEndpoints(app, endpointConfigurations);

Console.WriteLine("Running the application...");

app.Run();

Console.WriteLine("Application started. Ready to accept requests.");