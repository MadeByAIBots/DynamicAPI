using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.IO;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).FullName)
    .AddJsonFile("config.json")
    .Build();

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

var endpointLoader = new EndpointLoader(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "config"));
var endpointConfigurations = endpointLoader.LoadConfigurations();

var endpointExecutor = new EndpointExecutor();
endpointExecutor.CreateEndpoints(app, endpointConfigurations);

app.Run();