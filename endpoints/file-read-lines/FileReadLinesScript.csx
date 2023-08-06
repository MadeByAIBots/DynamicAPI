using System;
using System.IO;
using System.Threading.Tasks;
using DynamicApi.Contracts;
using DynamicApi.Endpoints.Model;
using Microsoft.Extensions.Logging;
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

		// ... existing code ...

		try
		{
			var text = File.ReadAllText(Path.Combine(workingDirectory, filePath));
			var formattedText = text.ToNumbered();

			return Task.FromResult(new EndpointExecutionResult
			{
				Body = formattedText,
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
}