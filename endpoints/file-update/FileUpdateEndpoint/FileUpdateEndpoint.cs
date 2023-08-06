using System.Diagnostics;
using System.Text;
using DynamicApi.Contracts;

namespace FileUpdateEndpoint;

public class FileUpdateEndpoint : DynamicEndpointExecutorBase
{
    public override async Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
        var workingDirectory = parameters.Parameters["workingDirectory"];
        var filePath = parameters.Parameters["filePath"];
        var content = parameters.Parameters["content"];

        // Parameter checks
        if (string.IsNullOrEmpty(workingDirectory))
        {
            return Fail("Error: The 'workingDirectory' parameter is null or empty.");
        }

        if (string.IsNullOrEmpty(filePath))
        {
            return Fail("Error: The 'filePath' parameter is null or empty.");
        }

        if (content == null)
        {
            return Fail("Error: The 'content' parameter is null.");
        }

        var fullPath = Path.Combine(workingDirectory, filePath);

        // File existence check
        if (!File.Exists(fullPath))
        {
            return Fail($"Error: The file '{fullPath}' does not exist.");
        }

        try
        {
            File.WriteAllText(fullPath, content);
        }
        catch (Exception ex)
        {
            return Fail($"Error: An error occurred while updating the file. Details: {ex.Message}");
        }

        // File update check
        var updatedFileContent = File.ReadAllText(fullPath);
        if (updatedFileContent != content)
        {
            return Fail("Error: The file was not updated correctly.");
        }

        return Success($"File updated successfully at {fullPath}.");
    }
}
