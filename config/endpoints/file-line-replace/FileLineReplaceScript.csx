using System;
using System.IO;
using System.Threading.Tasks;
using DynamicApi.Contracts;
using DynamicApiServer.Definitions.EndpointDefinitions;
using DynamicApi.Utilities.Files;

public class FileLineReplaceScriptEndpoint : IDynamicEndpointExecutor
{
    public Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
var workingDirectory = parameters.GetRequiredString("workingDirectory");
        var filePath = parameters.Parameters["filePath"];
        var lineNumber = Convert.ToInt32(parameters.Parameters["lineNumber"]);
        var newContent = parameters.Parameters["newContent"];
        var providedHash = parameters.Parameters["lineHash"];

        var fullPath = Path.Combine(workingDirectory, filePath);

        if (!File.Exists(fullPath))
        {
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = $"Error: The file '{fullPath}' does not exist.",
            });
        }

        var lines = File.ReadAllLines(fullPath);
        if (lineNumber < 1 || lineNumber > lines.Length)
        {
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = $"Error: Invalid line number {lineNumber}. The file has {lines.Length} lines.",
            });
        }

        var existingLine = lines[lineNumber - 1];
        var existingLineHash = HashUtils.GenerateSimpleHash(existingLine);
        if (existingLineHash != providedHash)
        {
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = "Error: Invalid hash. Read the lines to find out the correct hash and line number.",
            });
        }

        lines[lineNumber - 1] = newContent;
        File.WriteAllLines(fullPath, lines);

        return Task.FromResult(new EndpointExecutionResult
        {
            Body = "Line replaced successfully\nNew file content:\n" + File.ReadAllText(fullPath).ToNumbered(),
        });
    }
}
