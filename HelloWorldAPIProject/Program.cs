using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.IO;
using HelloWorldAPIProject.Definitions.EndpointDefinitions;
using HelloWorldAPIProject.Definitions.ExecutorDefinitions;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

var endpointLoader = new EndpointLoader(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "config/endpoints"));
var endpointConfigurations = endpointLoader.LoadConfigurations();

var endpointExecutor = new EndpointExecutor();
endpointExecutor.CreateEndpoints(app, endpointConfigurations, endpointLoader);

app.Run();