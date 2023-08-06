using System.Diagnostics;
using System.Text;
using DynamicApi.Contracts;

namespace FileCreateEndpoint;

public class FileCreateEndpoint : IDynamicEndpointExecutor
{
    public Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
        var workingDirectory = parameters.Parameters["workingDirectory"];
        var filePath = parameters.Parameters["filePath"];
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
