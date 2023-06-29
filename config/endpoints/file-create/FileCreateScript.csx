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
        if (!parameters.Parameters.ContainsKey("working-directory") ||
            !parameters.Parameters.ContainsKey("file-path") ||
            !parameters.Parameters.ContainsKey("content"))
        {
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = "Error: Missing required parameters. Please provide 'working-directory', 'file-path', and 'content'.",
                //StatusCode = 400
            });
        }

        var workingDirectory = parameters.Parameters["working-directory"];
        var filePath = parameters.Parameters["file-path"];
        var content = parameters.Parameters["content"];

        if (!Directory.Exists(workingDirectory))
        {
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = $"Error: The working directory '{workingDirectory}' does not exist.",
                //StatusCode = 400
            });
        }

        var fullPath = Path.Combine(workingDirectory, filePath);

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
            Body = "File created successfully.",
            //StatusCode = 200
        });
    }
}
