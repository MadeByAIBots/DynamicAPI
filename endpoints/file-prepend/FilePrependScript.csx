using System;
using System.IO;
using System.Threading.Tasks;
using DynamicApi.Contracts;
using DynamicApi.Endpoints.Model;

public class FilePrependScriptEndpoint : IDynamicEndpointExecutor
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

        var originalContent = File.ReadAllText(fullPath);
        var newContent = addNewline ? content + "\n" + originalContent : content + originalContent;
        File.WriteAllText(fullPath, newContent);

        return Task.FromResult(new EndpointExecutionResult
        {
            Body = "Content prepended successfully",
        });
    }
}