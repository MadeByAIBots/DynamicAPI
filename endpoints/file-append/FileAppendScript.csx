using System;
using System.IO;
using System.Threading.Tasks;
using DynamicApi.Contracts;
using DynamicApi.Endpoints.Model;

public class FileAppendScriptEndpoint : IDynamicEndpointExecutor
{
    public Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
        var workingDirectory = parameters.Parameters["workingDirectory"];
        var filePath = parameters.Parameters["filePath"];
        var content = parameters.Parameters["content"];
        var addNewline = parameters.Parameters.ContainsKey("addNewline") ? Convert.ToBoolean(parameters.Parameters["addNewline"]) : true;

        var fullPath = Path.Combine(workingDirectory, filePath);

        if (!File.Exists(fullPath))
        {
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = $"Error: The file '{fullPath}' does not exist.",
            });
        }

        var newContent = addNewline ? content + "\n" : content;
        File.AppendAllText(fullPath, newContent);

        return Task.FromResult(new EndpointExecutionResult
        {
            Body = "Content appended successfully",
        });
    }
}