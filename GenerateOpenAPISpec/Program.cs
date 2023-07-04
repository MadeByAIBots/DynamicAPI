using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace GenerateOpenAPISpec
{
    class Program
    {
        static void Main(string[] args)
        {
var host = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath);
        config.AddJsonFile("GenerateOpenAPISpec/appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddLogging(builder =>
        {
            builder.AddConfiguration(hostContext.Configuration.GetSection("Logging"));
            builder.AddConsole();
        });
        services.AddSingleton<OpenApiGenerator>();
services.AddSingleton<OperationProcessor>();
services.AddSingleton<ParameterProcessor>();
services.AddSingleton<DocumentGenerator>();
services.AddSingleton<EndpointProcessor>();
    })
    .Build();

var serviceProvider = host.Services;

            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            var endpointFileReaderLogger = serviceProvider.GetRequiredService<ILogger<EndpointFileReader>>();
            var openApiGenerator = serviceProvider.GetRequiredService<OpenApiGenerator>();

            logger.LogInformation("Application started.");

            try
            {
                var directory = "./config/endpoints";
                logger.LogInformation($"Reading endpoint definitions from directory '{directory}'...");

                var endpointFileReader = new EndpointFileReader(endpointFileReaderLogger);
                var endpointDefinitions = endpointFileReader.ReadEndpointFiles(directory);

                logger.LogInformation($"Generating OpenAPI specification from {endpointDefinitions.Count} endpoint definitions...");

                var openApiSpec = openApiGenerator.GenerateOpenApiSpec(endpointDefinitions);

                var outputFile = "DynamicApiServer/wwwroot/openapi.yaml";
                logger.LogInformation($"Writing OpenAPI specification to file '{outputFile}'...");

                File.WriteAllText(outputFile, openApiSpec);

                logger.LogInformation("Application finished successfully.");
Console.WriteLine("OpenAPI specification generated successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred: {ex.Message}");
            }
        }
    }
}

