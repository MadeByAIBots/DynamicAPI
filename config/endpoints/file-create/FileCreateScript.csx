#r "/root/workspace/DynamicAPI/DynamicApi.Contracts/bin/Debug/net7.0/DynamicApi.Contracts.dll"
#r "/root/workspace/DynamicAPI/Definitions/EndpointDefinitions/bin/Debug/net7.0/EndpointDefinitions.dll"

using System;
using System.IO;
using System.Threading.Tasks;
using DynamicApi.Contracts;
using DynamicApiServer.Definitions.EndpointDefinitions;

public class FileCreateScriptEndpoint : IDynamicEndpointExecutor
{
    public Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
        var workingDirectory = parameters.Parameters["working-directory"];
        var filePath = parameters.Parameters["file-path"];
        var content = parameters.Parameters["content"];

        var fullPath = Path.Combine(workingDirectory, filePath);
        var parentDirectory = Path.GetDirectoryName(fullPath);

        if (!Directory.Exists(parentDirectory))
        {
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = $"Error: The directory '{parentDirectory}' does not exist.",
                //StatusCode = 400
            });
        }

        if (File.Exists(fullPath))
        {
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = $"Error: The file '{fullPath}' already exists.",
                //StatusCode = 400
            });
        }

        try
        {
            File.WriteAllText(fullPath, content);
        }
        catch (Exception ex)
        {
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = $"Error: An error occurred while creating the file. Details: {ex.Message}",
                //StatusCode = 500
            });
        }

        return Task.FromResult(new EndpointExecutionResult
        {
            Body = $"File created successfully at {fullPath}.",
            //StatusCode = 200
        });
    }
}