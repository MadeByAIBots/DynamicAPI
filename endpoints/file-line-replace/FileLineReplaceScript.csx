using System;
using System.IO;
using System.Threading.Tasks;
using DynamicApi.Contracts;
using DynamicApi.Endpoints.Model;
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
                if (existingLineHash.ToLower() != providedHash.ToLower())
                {
			        return Fail(File.ReadAllText(fullPath).ToNumbered() + "\n\nError: Line hash and line number do not match. Verify and try again.");
                }

                lines[lineNumber - 1] = newContent;
                File.WriteAllLines(fullPath, lines);

                return Success("New file content:\n\n" + File.ReadAllText(fullPath).ToNumbered() + "\n\nLine replaced successfully");
        }
}