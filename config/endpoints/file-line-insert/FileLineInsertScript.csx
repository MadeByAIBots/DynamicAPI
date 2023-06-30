#r "/root/workspace/DynamicAPI/DynamicApi.Contracts/bin/Debug/net7.0/DynamicApi.Contracts.dll"
#r "/root/workspace/DynamicAPI/Definitions/EndpointDefinitions/bin/Debug/net7.0/EndpointDefinitions.dll"

using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using DynamicApi.Contracts;
using DynamicApiServer.Definitions.EndpointDefinitions;

public class FileLineInsertScriptEndpoint : IDynamicEndpointExecutor
{
    public Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
        var workingDirectory = parameters.Parameters["working-directory"];
        var filePath = parameters.Parameters["file-path"];
        var beforeLineNumber = int.Parse(parameters.Parameters["before-line-number"]);
        var newContent = parameters.Parameters["new-content"];

        // Parameter checks
        if (string.IsNullOrEmpty(workingDirectory))
        {
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = "Error: The 'working-directory' parameter is null or empty.",
                //StatusCode = 400
            });
        }

        if (string.IsNullOrEmpty(filePath))
        {
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = "Error: The 'file-path' parameter is null or empty.",
                //StatusCode = 400
            });
        }

        if (beforeLineNumber < 1)
        {
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = "Error: The 'before-line-number' parameter must be greater than 0.",
                //StatusCode = 400
            });
        }

        if (string.IsNullOrEmpty(newContent))
        {
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = "Error: The 'new-content' parameter is null or empty.",
                //StatusCode = 400
            });
        }

        var fullPath = Path.Combine(workingDirectory, filePath);

        // File existence check
        if (!File.Exists(fullPath))
        {
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = $"Error: The file '{fullPath}' does not exist.",
                //StatusCode = 400
            });
        }

        var lines = new List<string>(File.ReadAllLines(fullPath));

        // Check that beforeLineNumber is a valid line number in the file
        if (beforeLineNumber > lines.Count + 1)
        {
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = $"Error: The 'before-line-number' parameter is greater than the number of lines in the file plus 1.",
                //StatusCode = 400
            });
        }

        // Subtract 1 from beforeLineNumber to get the correct index
        lines.Insert(beforeLineNumber - 1, newContent);

        File.WriteAllLines(fullPath, lines);

        return Task.FromResult(new EndpointExecutionResult
        {
            Body = "Line inserted successfully",
            //StatusCode = 200
        });
    }
}