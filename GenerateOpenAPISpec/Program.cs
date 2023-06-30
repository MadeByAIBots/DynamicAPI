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
            using var serviceProvider = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Debug);
                })
                .AddTransient<EndpointProcessor>()
                .AddTransient<OperationProcessor>()
                .AddTransient<ParameterProcessor>()
                .AddTransient<DocumentGenerator>()
                .AddTransient<OpenApiGenerator>()
                .BuildServiceProvider();

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
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred: {ex.Message}");
            }
        }
    }
}