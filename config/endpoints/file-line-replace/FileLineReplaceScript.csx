using System;
using System.IO;
using System.Threading.Tasks;
using DynamicApi.Contracts;
using DynamicApiServer.Definitions.EndpointDefinitions;
using DynamicApi.Utilities.Files;

public class FileLineReplaceScriptEndpoint : DynamicEndpointExecutorBase
{
public override async Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
var workingDirectory = parameters.GetRequiredString("workingDirectory");
var filePath = parameters.GetRequiredString("filePath");
var lineNumber = parameters.GetRequiredInt32("lineNumber");
var newContent = parameters.GetRequiredString("newContent");
var providedHash = parameters.GetRequiredString("lineHash");

        var fullPath = Path.Combine(workingDirectory, filePath);

        if (!File.Exists(fullPath))
        {
return Fail($"Error: The file '{fullPath}' does not exist.");
        }

        var lines = File.ReadAllLines(fullPath);
        if (lineNumber < 1 || lineNumber > lines.Length)
        {
return Fail($"Error: Invalid line number {lineNumber}. The file has {lines.Length} lines.");
        }

        var existingLine = lines[lineNumber - 1];
        var existingLineHash = HashUtils.GenerateSimpleHash(existingLine);
        if (existingLineHash != providedHash)
        {
return Fail("Error: Invalid hash. Read the lines to find out the correct hash and line number.");
        }

        lines[lineNumber - 1] = newContent;
        File.WriteAllLines(fullPath, lines);

return Success("Line replaced successfully\nNew file content:\n" + File.ReadAllText(fullPath).ToNumbered());
    }
}
