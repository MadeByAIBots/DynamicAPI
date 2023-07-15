using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DynamicApi.Contracts;
using DynamicApiServer.Definitions.EndpointDefinitions;
using DynamicApi.Utilities.Files;

public class FileLinesRangeReplaceScriptEndpoint : IDynamicEndpointExecutor
{
    public Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
        var workingDirectory = parameters.Parameters["workingDirectory"];
        var filePath = parameters.Parameters["filePath"];
        var startLineNumber = Convert.ToInt32(parameters.Parameters["startLineNumber"]);
        var endLineNumber = Convert.ToInt32(parameters.Parameters["endLineNumber"]);
        var newContents = parameters.Parameters["newContents"];
        var startLineHash = parameters.Parameters["startLineHash"];
        var endLineHash = parameters.Parameters["endLineHash"];

        var fullPath = Path.Combine(workingDirectory, filePath);

        if (!File.Exists(fullPath))
        {
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = $"Error: The file '{fullPath}' does not exist.",
            });
        }

        var lines = File.ReadAllLines(fullPath);
        if (startLineNumber < 1 || startLineNumber > lines.Length || endLineNumber < 1 || endLineNumber > lines.Length)
        {
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = $"Error: Invalid line numbers. The file has {lines.Length} lines.",
            });
        }

        var startLine = lines[startLineNumber - 1];
        var endLine = lines[endLineNumber - 1];
        var startLineHashCheck = HashUtils.GenerateSimpleHash(startLine);
        var endLineHashCheck = HashUtils.GenerateSimpleHash(endLine);
        if (startLineHashCheck.ToLower() != startLineHash.ToLower() || endLineHashCheck.ToLower() != endLineHash.ToLower())
        {
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = "Error: Invalid hash. Read the lines to find out the correct hash and line number.",
            });
        }

        // Remove the specified range of lines
        var rangeLength = endLineNumber - startLineNumber + 1;
        lines = lines.Take(startLineNumber - 1).Concat(lines.Skip(endLineNumber)).ToArray();

        // Insert the new content at the start line position
        var newContentLines = newContents.Split('\n');
        lines = lines.Take(startLineNumber - 1).Concat(newContentLines).Concat(lines.Skip(startLineNumber - 1)).ToArray();

        File.WriteAllLines(fullPath, lines);

        return Task.FromResult(new EndpointExecutionResult
        {
            Body = "Line(s) replaced successfully\nNew file content:\n" + File.ReadAllText(fullPath).ToNumbered(),
        });
    }
}