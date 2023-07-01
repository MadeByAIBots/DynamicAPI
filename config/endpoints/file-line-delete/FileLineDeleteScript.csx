#r "/root/workspace/DynamicAPI/DynamicApi.Contracts/bin/Debug/net7.0/DynamicApi.Contracts.dll"
#r "/root/workspace/DynamicAPI/Definitions/EndpointDefinitions/bin/Debug/net7.0/EndpointDefinitions.dll"

#r "/root/workspace/DynamicAPI/Utilities/Files/bin/Debug/net7.0/DynamicApi.Utilities.Files.dll"
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using DynamicApi.Contracts;
using DynamicApiServer.Definitions.EndpointDefinitions;
using DynamicApi.Utilities.Files;

public class FileLineDeleteScriptEndpoint : IDynamicEndpointExecutor
{
	public Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
	{
		var workingDirectory = parameters.Parameters["workingDirectory"];
		var filePath = parameters.Parameters["filePath"];
		var providedHash = parameters.Parameters["lineHash"];
		var lineNumber = int.Parse(parameters.Parameters["lineNumber"]);

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

		var existingLine = lines[lineNumber - 1];
		var existingLineHash = HashUtils.GenerateSimpleHash(existingLine);
		if (existingLineHash != providedHash)
		{
			return Task.FromResult(new EndpointExecutionResult
			{
				Body = "Error: Invalid hash. Read the lines to find out the correct hash and line number.",
			});
		}
		if (lineNumber < 1 || lineNumber > lines.Count)
		{
			return Task.FromResult(new EndpointExecutionResult
			{
				Body = "Error: The 'lineNumber' parameter is out of range.",
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
