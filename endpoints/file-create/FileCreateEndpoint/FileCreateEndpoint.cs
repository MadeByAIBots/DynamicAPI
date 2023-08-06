using System.Diagnostics;
using System.Text;
using DynamicApi.Contracts;

namespace FileCreateEndpoint;

public class FileCreateEndpoint : DynamicEndpointExecutorBase
{
    public override async Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
        var workingDirectory = parameters.GetRequiredString("workingDirectory");
        var filePath = parameters.GetRequiredString("filePath");
        var content = parameters.GetRequiredString("content");

        var fullPath = Path.Combine(workingDirectory, filePath);
        var parentDirectory = Path.GetDirectoryName(fullPath);

        if (!Directory.Exists(parentDirectory))
        {
            return Fail($"Error: The directory '{parentDirectory}' does not exist.");
        }

        if (File.Exists(fullPath))
        {
            return Fail($"Error: The file '{fullPath}' already exists.");
        }

        try
        {
            File.WriteAllText(fullPath, content);
        }
        catch (Exception ex)
        {
            return Fail($"Error: An error occurred while creating the file. Details: {ex.Message}");
        }

        return Success($"File created successfully at {fullPath}.");
    }
}
