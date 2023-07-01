using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using DynamicApi.Contracts;
using DynamicApiServer.Definitions.EndpointDefinitions;

public class FileLineInsertScriptEndpoint : IDynamicEndpointExecutor
{
	public Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
	{
		var workingDirectory = parameters.Parameters["workingDirectory"];
		var filePath = parameters.Parameters["filePath"];
		var beforeLineNumber = int.Parse(parameters.Parameters["beforeLineNumber"]);
		var lineHash = parameters.Parameters["beforeLineHash"];
		var newContent = parameters.Parameters["newContent"];

		// Parameter checks
		if (string.IsNullOrEmpty(workingDirectory))
		{
			return Task.FromResult(new EndpointExecutionResult
			{
				Body = "Error: The 'workingDirectory' parameter is null or empty.",
				//StatusCode = 400
			});
		}

		if (string.IsNullOrEmpty(filePath))
		{
			return Task.FromResult(new EndpointExecutionResult
			{
				Body = "Error: The 'filePath' parameter is null or empty.",
				//StatusCode = 400
			});
		}

		if (beforeLineNumber < 1)
		{
			return Task.FromResult(new EndpointExecutionResult
			{
				Body = "Error: The 'beforeLineNumber' parameter must be greater than 0.",
				//StatusCode = 400
			});
		}

		if (string.IsNullOrEmpty(newContent))
		{
			return Task.FromResult(new EndpointExecutionResult
			{
				Body = "Error: The 'newContent' parameter is null or empty.",
				//StatusCode = 400
			});
		}

		var fullPath = Path.Combine(workingDirectory, filePath);

		// File existence check
		if (!File.Exists(fullPath))
		{
			return Task.FromResult(new EndpointExecutionResult
			{
				Body = $"Error: The file '{fullPath}' does not exist.",
				//StatusCode = 400
			});
		}

		var lines = new List<string>(File.ReadAllLines(fullPath));

		// Check that beforeLineNumber is a valid line number in the file
		if (beforeLineNumber > lines.Count + 1)
		{
			return Task.FromResult(new EndpointExecutionResult
			{
				Body = $"Error: The 'beforeLineNumber' parameter is greater than the number of lines in the file plus 1.",
				//StatusCode = 400
			});
		}

		// Hash check
		var generatedHash = DynamicApi.Utilities.Files.HashUtils.GenerateSimpleHash(lines[beforeLineNumber - 1]);
		if (generatedHash != lineHash)
		{
			return Task.FromResult(new EndpointExecutionResult
			{
				Body = "Error: Invalid hash. Read the lines to find out the correct hash and line number.",
			});
		}


		// Subtract 1 from beforeLineNumber to get the correct index
		lines.Insert(beforeLineNumber - 1, newContent);

		File.WriteAllLines(fullPath, lines);

		return Task.FromResult(new EndpointExecutionResult
		{
			Body = "Line inserted successfully",
			//StatusCode = 200
		});
	}
}
