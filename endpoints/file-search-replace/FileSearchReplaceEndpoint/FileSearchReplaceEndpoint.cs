using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using DynamicApi.Contracts;
using DynamicApi.Utilities.Files;

namespace FileSearchReplaceEndpoint;

public class FileSearchReplaceEndpoint : DynamicEndpointExecutorBase
{
    public override async Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
        var workingDirectory = parameters.GetRequiredString("workingDirectory");
        var filePath = parameters.GetRequiredString("filePath");
        var searchQuery = parameters.GetRequiredString("searchQuery");
        var replacementString = parameters.GetRequiredString("replacementString");

        var fullPath = Path.Combine(workingDirectory, filePath);

        if (!File.Exists(fullPath))
        {
            return Fail($"Error: The file '{fullPath}' does not exist.");
        }

        try
        {
            var fileContent = File.ReadAllText(fullPath);
            var matchCount = Regex.Matches(fileContent, Regex.Escape(searchQuery)).Count;
            var newContent = fileContent.Replace(searchQuery, replacementString);
            
            File.WriteAllText(fullPath, newContent);

            return Success($"Search and replace operation completed successfully. Replaced {matchCount} instances.\nNew file content:\n{newContent}");
        }
        catch (Exception ex)
        {
            return Fail($"Error: An error occurred while performing the search and replace operation. Details: {ex.Message}");
        }
    }
}
