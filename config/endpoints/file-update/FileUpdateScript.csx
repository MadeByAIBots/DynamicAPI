#r "/root/workspace/DynamicAPI/DynamicApi.Contracts/bin/Debug/net7.0/DynamicApi.Contracts.dll"
#r "/root/workspace/DynamicAPI/Definitions/EndpointDefinitions/bin/Debug/net7.0/EndpointDefinitions.dll"

using System;
using System.IO;
using System.Threading.Tasks;
using DynamicApi.Contracts;
using DynamicApiServer.Definitions.EndpointDefinitions;

public class FileUpdateScriptEndpoint : IDynamicEndpointExecutor
{
    public Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
        var workingDirectory = parameters.Parameters["workingDirectory"];
        var filePath = parameters.Parameters["filePath"];
        var content = parameters.Parameters["content"];

        // Parameter checks
        if (string.IsNullOrEmpty(workingDirectory))
        {
            Console.WriteLine("Error: The 'workingDirectory' parameter is null or empty.");
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = "Error: The 'workingDirectory' parameter is null or empty.",
                //StatusCode = 400
            });
        }

        if (string.IsNullOrEmpty(filePath))
        {
            Console.WriteLine("Error: The 'filePath' parameter is null or empty.");
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = "Error: The 'filePath' parameter is null or empty.",
                //StatusCode = 400
            });
        }

        if (content == null)
        {
            Console.WriteLine("Error: The 'content' parameter is null.");
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = "Error: The 'content' parameter is null.",
                //StatusCode = 400
            });
        }

        var fullPath = Path.Combine(workingDirectory, filePath);

        // File existence check
        if (!File.Exists(fullPath))
        {
            Console.WriteLine($"Error: The file '{fullPath}' does not exist.");
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = $"Error: The file '{fullPath}' does not exist.",
                //StatusCode = 400
            });
        }

        try
        {
            File.WriteAllText(fullPath, content);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: An error occurred while updating the file. Details: {ex.Message}");
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = $"Error: An error occurred while updating the file. Details: {ex.Message}",
                //StatusCode = 500
            });
        }

        // File update check
        var updatedFileContent = File.ReadAllText(fullPath);
        if (updatedFileContent != content)
        {
            Console.WriteLine("Error: The file was not updated correctly.");
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = "Error: The file was not updated correctly.",
                //StatusCode = 500
            });
        }

        return Task.FromResult(new EndpointExecutionResult
        {
            Body = $"File updated successfully at {fullPath}.",
            //StatusCode = 200
        });
    }
}
