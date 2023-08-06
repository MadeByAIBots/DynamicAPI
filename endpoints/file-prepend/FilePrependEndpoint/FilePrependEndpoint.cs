using System.Diagnostics;
using System.Text;
using DynamicApi.Contracts;
using DynamicApi.Utilities.Files;
using Microsoft.Extensions.Logging;

namespace FilePrependEndpoint;

public class FilePrependEndpoint : DynamicEndpointExecutorBase
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

        var originalContent = File.ReadAllText(fullPath);
        var newContent = addNewline ? content + "\n" + originalContent : content + originalContent;
        File.WriteAllText(fullPath, newContent);

        return Success("Content prepended successfully");
    }
}
