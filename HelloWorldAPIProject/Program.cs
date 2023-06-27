using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using HelloWorldAPIProject.Extensions;
using HelloWorldAPIProject.Definitions.ExecutorDefinitions;

Console.WriteLine("[INFO] Starting application...");

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddSingleton<EndpointService>(sp => new EndpointService("/root/workspace/DynamicAPI/config/endpoints"));
builder.Services.AddSingleton<RequestProcessor>();
builder.Services.AddSingleton<EndpointExecutor>();

Console.WriteLine("[INFO] EndpointExecutor would be initialized here.");

var app = builder.BuildConfiguredApplication();

Console.WriteLine("[INFO] Web application created and configured.");

// Use the request logging middleware
app.UseRequestLogging();

// Use the dynamic endpoint middleware
app.UseDynamicEndpoints();

// Map the fallback route
app.MapFallbackRoute();

Console.WriteLine("[INFO] Endpoints mapped.");

Console.WriteLine("[INFO] Application is now listening for requests...");

app.Run();