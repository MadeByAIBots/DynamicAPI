#r "/root/workspace/DynamicAPI/DynamicApi.Contracts/bin/Debug/net7.0/DynamicApi.Contracts.dll"
#r "/root/workspace/DynamicAPI/Definitions/EndpointDefinitions/bin/Debug/net7.0/EndpointDefinitions.dll"

using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using DynamicApi.Contracts;
using DynamicApiServer.Definitions.EndpointDefinitions;

public class FileLineDeleteScriptEndpoint : IDynamicEndpointExecutor
{
    public Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
        var workingDirectory = parameters.Parameters["workingDirectory"];
        var filePath = parameters.Parameters["file-path"];
        var lineNumber = int.Parse(parameters.Parameters["line-number"]);

        var fullPath = Path.Combine(workingDirectory, filePath);

        if (!File.Exists(fullPath))
        {
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = $"Error: The file '{fullPath}' does not exist.",
                //StatusCode = 400
            });
        }

        var lines = new List<string>(File.ReadAllLines(fullPath));

        if (lineNumber < 1 || lineNumber > lines.Count)
        {
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = "Error: The 'line-number' parameter is out of range.",
                //StatusCode = 400
            });
        }

        lines.RemoveAt(lineNumber - 1);
        File.WriteAllLines(fullPath, lines);

        return Task.FromResult(new EndpointExecutionResult
        {
            Body = "Line deleted successfully",
            //StatusCode = 200
        });
    }
}