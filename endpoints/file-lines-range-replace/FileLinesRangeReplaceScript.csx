using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DynamicApi.Contracts;
using DynamicApi.Endpoints.Model;
using DynamicApi.Utilities.Files;

public class FileLinesRangeReplaceScriptEndpoint : DynamicEndpointExecutorBase
{
    public override async Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
        var workingDirectory = parameters.GetRequiredString("workingDirectory");
        var filePath = parameters.GetRequiredString("filePath");
        var startLineNumber = parameters.GetRequiredInt32("startLineNumber");
        var endLineNumber = parameters.GetRequiredInt32("endLineNumber");
        var newContents = parameters.GetRequiredString("newContents");
        var startLineHash = parameters.GetRequiredString("startLineHash");
        var endLineHash = parameters.GetRequiredString("endLineHash");

        var fullPath = Path.Combine(workingDirectory, filePath);

        if (!File.Exists(fullPath))
        {
            return Fail($"Error: The file '{fullPath}' does not exist.");
        }

        var lines = File.ReadAllLines(fullPath);
        if (startLineNumber < 1 || startLineNumber > lines.Length || endLineNumber < 1 || endLineNumber > lines.Length)
        {
            return Fail($"Error: Invalid line numbers. The file has {lines.Length} lines.");
        }

        var startLine = lines[startLineNumber - 1];
        var endLine = lines[endLineNumber - 1];
        var startLineHashCheck = HashUtils.GenerateSimpleHash(startLine);
        var endLineHashCheck = HashUtils.GenerateSimpleHash(endLine);
        if (startLineHashCheck.ToLower() != startLineHash.ToLower() || endLineHashCheck.ToLower() != endLineHash.ToLower())
        {
            return Fail(File.ReadAllText(fullPath).ToNumbered() + "\n\nError: Line hashes and line numbers do not match. Verify and try again.");
        }

        // Remove the specified range of lines
        var rangeLength = endLineNumber - startLineNumber + 1;
        lines = lines.Take(startLineNumber - 1).Concat(lines.Skip(endLineNumber)).ToArray();

        // Insert the new content at the start line position
        var newContentLines = newContents.Split('\n');
        lines = lines.Take(startLineNumber - 1).Concat(newContentLines).Concat(lines.Skip(startLineNumber - 1)).ToArray();

        File.WriteAllLines(fullPath, lines);

        return Success("New file content:\n\n" + File.ReadAllText(fullPath).ToNumbered() + "\n\nLine(s) replaced successfully");
    }
}