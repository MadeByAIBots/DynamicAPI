using System.Diagnostics;
using System.Text;
using DynamicApi.Contracts;
using DynamicApi.Utilities.Files;
using Microsoft.Extensions.Logging;

namespace FileReadLinesEndpoint;

public class FileReadLinesEndpoint : DynamicEndpointExecutorBase
{
	private ILogger<FileReadLinesEndpoint> logger;

	public override async Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
	{
		this.logger = new Logger<FileReadLinesEndpoint>(parameters.LoggerFactory);
		var workingDirectory = parameters.Parameters["workingDirectory"];
		var filePath = parameters.Parameters["filePath"];

		try
		{
			var text = File.ReadAllText(Path.Combine(workingDirectory, filePath));
			var formattedText = text.ToNumbered();

			return Success(formattedText);
		}
		catch (Exception ex)
		{
			return Fail($"Error: An error occurred while reading the file. Details: {ex.ToString()}");
		}
	}
}
