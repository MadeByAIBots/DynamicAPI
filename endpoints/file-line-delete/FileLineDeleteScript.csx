using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using DynamicApi.Contracts;
using DynamicApi.Endpoints.Model;
using DynamicApi.Utilities.Files;

public class FileLineDeleteScriptEndpoint : DynamicEndpointExecutorBase
{
	public override async Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
	{
		var workingDirectory = parameters.Parameters["workingDirectory"];
		var filePath = parameters.Parameters["filePath"];
		var providedHash = parameters.Parameters["lineHash"];
		var lineNumber = int.Parse(parameters.Parameters["lineNumber"]);

		var fullPath = Path.Combine(workingDirectory, filePath);

		if (!File.Exists(fullPath))
		{
			return Fail($"Error: The file '{fullPath}' does not exist.");
		}

		var lines = new List<string>(File.ReadAllLines(fullPath));

		var existingLine = lines[lineNumber - 1];
		var existingLineHash = HashUtils.GenerateSimpleHash(existingLine);
		if (existingLineHash.ToLower() != providedHash.ToLower())
		{
			return Fail(File.ReadAllText(fullPath).ToNumbered() + "\n\nError: Line hash and line number do not match. Verify and try again.");
		}
		if (lineNumber < 1 || lineNumber > lines.Count)
		{
			return Fail("Error: The 'lineNumber' parameter is out of range.");
		}

		lines.RemoveAt(lineNumber - 1);
		File.WriteAllLines(fullPath, lines);

		return Success("New file content:\n\n" + File.ReadAllText(fullPath).ToNumbered() + "\n\nLine deleted successfully");
	}
}