#r "/root/workspace/DynamicAPI/DynamicApi.Contracts/bin/Debug/net7.0/DynamicApi.Contracts.dll"
#r "/root/workspace/DynamicAPI/Definitions/EndpointDefinitions/bin/Debug/net7.0/EndpointDefinitions.dll"

using System;
using System.IO;
using System.Threading.Tasks;
using DynamicApi.Contracts;
using DynamicApiServer.Definitions.EndpointDefinitions;

public class FileLineReplaceScriptEndpoint : IDynamicEndpointExecutor
{
    public Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
        var workingDirectory = parameters.Parameters["workingDirectory"];
        var filePath = parameters.Parameters["filePath"];
        var lineNumber = Convert.ToInt32(parameters.Parameters["lineNumber"]);
        var newContent = parameters.Parameters["newContent"];

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

        lines[lineNumber - 1] = newContent;
        File.WriteAllLines(fullPath, lines);

        return Task.FromResult(new EndpointExecutionResult
        {
            Body = "Line replaced successfully",
        });
    }
}