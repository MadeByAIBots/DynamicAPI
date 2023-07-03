using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DynamicApi.Contracts;
using DynamicApiServer.Definitions.EndpointDefinitions;

public class FileSearchReplaceScriptEndpoint : IDynamicEndpointExecutor
{
    public Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
        var workingDirectory = parameters.Parameters["workingDirectory"];
        var filePath = parameters.Parameters["filePath"];
        var searchQuery = parameters.Parameters["searchQuery"];
        var replacementString = parameters.Parameters["replacementString"];

        var fullPath = Path.Combine(workingDirectory, filePath);

        if (!File.Exists(fullPath))
        {
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = $"Error: The file '{fullPath}' does not exist.",
            });
        }

        try
        {
            var fileContent = File.ReadAllText(fullPath);
            var matchCount = Regex.Matches(fileContent, Regex.Escape(searchQuery)).Count;
            var newContent = fileContent.Replace(searchQuery, replacementString);
            File.WriteAllText(fullPath, newContent);

            return Task.FromResult(new EndpointExecutionResult
            {
                Body = $"Search and replace operation completed successfully. Replaced {matchCount} instances.\nNew file content:\n{newContent}",
            });
        }
        catch (Exception ex)
        {
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = $"Error: An error occurred while performing the search and replace operation. Details: {ex.Message}",
            });
        }
    }
}