using System.Diagnostics;
using System.Text;
using DynamicApi.Contracts;
using DynamicApi.Utilities.Files;

namespace FileReadMultipleEndpoint;

public class FileReadMultipleEndpoint : DynamicEndpointExecutorBase
{
	public override async Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
	{
		var workingDirectory = parameters.GetRequiredString("workingDirectory");
		var filePathsString = parameters.GetRequiredString("filePaths");

		var filePaths = filePathsString.Split(';');

		var outputBuilder = new StringBuilder();
		foreach (var filePath in filePaths)
		{
			var fullPath = Path.Combine(workingDirectory, filePath);

			if (!File.Exists(fullPath))
			{
				return Fail($"Error: The file '{fullPath}' does not exist.");
			}

			var relativePath = fullPath.Replace(workingDirectory, "");
			outputBuilder.AppendLine($"[{relativePath}]");
			
			var fileContent = File.ReadAllText(fullPath);
			outputBuilder.AppendLine(fileContent);

			outputBuilder.AppendLine();
			outputBuilder.AppendLine();
		}

		return Success(outputBuilder.ToString());
	}
}
