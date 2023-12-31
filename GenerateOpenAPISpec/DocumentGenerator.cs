using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Microsoft.Extensions.Logging;

public class DocumentGenerator
{
    private readonly ILogger<DocumentGenerator> _logger;

    public DocumentGenerator(ILogger<DocumentGenerator> logger)
    {
        _logger = logger;
    }

    public string GenerateDocument(Dictionary<string, OpenApiPathItem> paths)
    {
        if (paths == null)
        {
            _logger.LogError("Paths dictionary is null.");
            throw new ArgumentNullException(nameof(paths));
        }

        _logger.LogInformation("Generating OpenAPI document...");

        var openApiPaths = new OpenApiPaths();
        foreach (var path in paths)
        {
            openApiPaths.Add(path.Key, path.Value);
        }

        var document = new OpenApiDocument
        {
            Info = new OpenApiInfo
            {
                Title = "DynamicAPI Self-Evolving Endpoints",
                Description = "DynamicAPI is a self-evolving API service that allows for the dynamic generation of endpoints from JSON files. With the ability to create, list, and execute endpoints, DynamicAPI provides a flexible and efficient solution for expanding API functionality. Leveraging the power of ChatGPT, DynamicAPI aims to streamline the process of endpoint creation, making API development faster and more efficient.\n\nInstructions:\nWhen a message is provided by the user, output the following headings with detailed information below each...\nAnalyse\nPlan\nReview plan\nExecute\nIMPORTANT: Only carry out an action AFTER confirming with the user.",
                Version = "1.0.0"
            },
            Paths = openApiPaths
        };
        document.Servers.Add(new OpenApiServer{Url="{{SERVER_URL}}"});

        var writer = new StringWriter();
        var yamlWriter = new OpenApiYamlWriter(writer);

        document.SerializeAsV3(yamlWriter);

        var openApiSpec = writer.ToString();

        _logger.LogInformation("Finished generating OpenAPI document.");

        return openApiSpec;
    }
}