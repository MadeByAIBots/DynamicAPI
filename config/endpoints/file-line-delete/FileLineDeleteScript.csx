using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using DynamicApi.Contracts;
using DynamicApiServer.Definitions.EndpointDefinitions;
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
		if (existingLineHash != providedHash)
		{
			return Fail("Error: Invalid hash. Read the lines to find out the correct hash and line number.");
		}
		if (lineNumber < 1 || lineNumber > lines.Count)
		{
			return Fail("Error: The 'lineNumber' parameter is out of range.");
		}

		lines.RemoveAt(lineNumber - 1);
		File.WriteAllLines(fullPath, lines);

		return Success("Line deleted successfully\nNew file content:\n" + File.ReadAllText(fullPath).ToNumbered());
	}
}