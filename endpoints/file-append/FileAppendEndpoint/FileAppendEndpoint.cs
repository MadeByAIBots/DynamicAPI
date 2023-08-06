using System.Diagnostics;
using System.Text;
using DynamicApi.Contracts;
using DynamicApi.Utilities.Files;
using Microsoft.Extensions.Logging;

namespace FileAppendEndpoint;

public class FileAppendEndpoint : DynamicEndpointExecutorBase
{
    public override async Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
        var workingDirectory = parameters.GetRequiredString("workingDirectory");
        var filePath = parameters.GetRequiredString("filePath");
        var content = parameters.GetRequiredString("content");
        var addNewline = parameters.GetBool("addNewline", true);

        var fullPath = Path.Combine(workingDirectory, filePath);

        if (!File.Exists(fullPath))
        {
            return Fail($"Error: The file '{fullPath}' does not exist.");
        }

        var newContent = addNewline ? content + "\n" : content;
        File.AppendAllText(fullPath, newContent);

        return Success("Content appended successfully");
    }
}
