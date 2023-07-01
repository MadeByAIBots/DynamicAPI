#r "/root/workspace/DynamicAPI/DynamicApi.Contracts/bin/Debug/net7.0/DynamicApi.Contracts.dll"
#r "/root/workspace/DynamicAPI/Definitions/EndpointDefinitions/bin/Debug/net7.0/EndpointDefinitions.dll"
#r "/root/workspace/DynamicAPI/Utilities/Files/bin/Debug/net7.0/DynamicApi.Utilities.Files.dll"

using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DynamicApi.Contracts;
using DynamicApiServer.Definitions.EndpointDefinitions;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using DynamicApi.Utilities.Files;

public class FileReadLinesScriptEndpoint : IDynamicEndpointExecutor
{
	private ILogger<FileReadLinesScriptEndpoint> logger;

	public FileReadLinesScriptEndpoint()
	{
	}

	public Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
	{
		this.logger = new Logger<FileReadLinesScriptEndpoint>(parameters.LoggerFactory);
		var workingDirectory = parameters.Parameters["workingDirectory"];
		var filePath = parameters.Parameters["filePath"];

		// Parameter checks
		if (string.IsNullOrEmpty(workingDirectory))
		{
			Console.WriteLine("Error: The 'workingDirectory' parameter is null or empty.");
			return Task.FromResult(new EndpointExecutionResult
			{
				Body = "Error: The 'workingDirectory' parameter is null or empty.",
				//StatusCode = 400
			});
		}

		if (string.IsNullOrEmpty(filePath))
		{
			Console.WriteLine("Error: The 'filePath' parameter is null or empty.");
			return Task.FromResult(new EndpointExecutionResult
			{
				Body = "Error: The 'filePath' parameter is null or empty.",
				//StatusCode = 400
			});
		}

		var fullPath = Path.Combine(workingDirectory, filePath);

		// File existence check
		if (!File.Exists(fullPath))
		{
			Console.WriteLine($"Error: The file '{fullPath}' does not exist.");
			return Task.FromResult(new EndpointExecutionResult
			{
				Body = $"Error: The file '{fullPath}' does not exist.",
				//StatusCode = 400
			});
		}

		try
		{
			var lines = File.ReadAllLines(fullPath);
			var formattedLines = new List<string>();

			for (int i = 0; i < lines.Length; i++)
			{
				var hash = HashUtils.GenerateSimpleHash(lines[i]);
				formattedLines.Add(FormatLine(hash, i + 1, lines[i]));
			}

			return Task.FromResult(new EndpointExecutionResult
			{
				Body = string.Join("\n", formattedLines),
				//StatusCode = 200
			});
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error: An error occurred while reading the file. Details: {ex.Message}");
			return Task.FromResult(new EndpointExecutionResult
			{
				Body = $"Error: An error occurred while reading the file. Details: {ex.Message}",
				//StatusCode = 500
			});
		}
	}



	private string FormatLine(string hash, int lineNumber, string lineContent)
	{
		return $"[{hash}] {lineNumber}: {lineContent}";
	}
}