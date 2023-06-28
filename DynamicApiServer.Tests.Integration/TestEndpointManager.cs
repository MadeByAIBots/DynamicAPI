using System;
using System.IO;
using System.Text.Json;
using DynamicApiServer.Definitions.EndpointDefinitions;
using DynamicApiServer.Definitions.ExecutorDefinitions;
using Microsoft.Extensions.Logging;

namespace DynamicApiServer.Tests.Integration;

public class TestEndpointManager : IDisposable
{
    private string _baseDir;
    private string _endpointName;
    private string _endpointDirectory;
    private ILogger<TestEndpointManager> _logger;

    public TestEndpointManager(string baseDir, ILoggerFactory loggerFactory)
    {
        _baseDir = baseDir;
        _logger = loggerFactory.CreateLogger<TestEndpointManager>();
    }

    public TestEndpointManager Create(string baseEndpointName)
    {
        try
        {
            _logger.LogInformation("Creating test endpoint...");

            _logger.LogInformation($"Base dir: {_baseDir}");

            // Generate a unique ID
            string uniqueId = Guid.NewGuid().ToString().Substring(0, 9).TrimEnd('-');

            // Create the endpoint directory
            _endpointName = baseEndpointName + "-" + uniqueId;

            _logger.LogInformation($"Name: {_endpointName}");

            _endpointDirectory = _baseDir + "/" + _endpointName.Trim('/');

            _logger.LogInformation($"Path: {_endpointDirectory}");


            // Create the directory
            Directory.CreateDirectory(_endpointDirectory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }


        return this;
    }

    public TestEndpointManager AddFile(string fileName, string content)
    {
        // Create the file in the endpoint directory
        string filePath = Path.Combine(_endpointDirectory, fileName);

        // Write the file
        File.WriteAllText(filePath, content);

        return this;
    }

    public TestEndpointManager AddFile(object contentObj)
    {
        // Determine the file name based on the type of the object
        string fileName;
        if (contentObj is EndpointDefinition)
        {
            fileName = "endpoint.json";

            // Set the temporary endpoint path/name to the endpoint definition because a unique ID is appended to the name it starts with so that it's unique for testing
            var endpointDefinition = (EndpointDefinition)contentObj;
            endpointDefinition.Path = "/" + _endpointName.TrimStart('/');
            endpointDefinition.FolderName = _endpointName;
        }
        else if (contentObj is BashExecutorDefinition)
        {
            fileName = "bash.json";
        }
        else
        {
            throw new ArgumentException("Invalid object type");
        }

        // Serialize the object to a JSON string
        string content = JsonSerializer.Serialize(contentObj);

        AddFile(fileName, content);

        return this;
    }

    public TestEndpointManager AddBashEndpoint(string command, string method)
    {
        AddFile(
            new EndpointDefinition
            {
                Executor = "bash",
                Method = method
            }
        );

        AddFile(
            new BashExecutorDefinition
            {
                Command = command
            }
        );

        return this;

    }

    public string GetEndpointPath()
    {
        // Return the unique endpoint path
        return _endpointName;
    }

    public void Dispose()
    {
        // Delete the temporary test endpoint directory
        string testEndpointPath = Path.Combine(_baseDir, _endpointName);
        Directory.Delete(testEndpointPath, true);
    }
}