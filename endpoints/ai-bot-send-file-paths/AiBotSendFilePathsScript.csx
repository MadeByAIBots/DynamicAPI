using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using DynamicApi.Contracts;
using TalkFlow.Messages.Core.Provider;
using TalkFlow.Messages.Model;
using TalkFlow.Messages.Providers.Backend.OpenAI;

public class AiSendFilePathsScriptEndpoint : DynamicEndpointExecutorBase
{
	public override async Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
	{
		if (!parameters.ApiConfig.Variables.TryGetValue("OpenAIKey", out var apiKey))
		{
			return Fail("OpenAI API key not found in configuration.");
		}

		var workingDirectory = parameters.GetRequiredString("workingDirectory");
		var directory = parameters.GetRequiredString("subDirectory");
		var message = parameters.GetRequiredString("message");
		var recursive = parameters.GetRequiredBool("recursive");

		var absoluteDirectory = Path.Combine(workingDirectory, directory);

		if (!Directory.Exists(absoluteDirectory))
		{
			return Fail($"Directory does not exist: {absoluteDirectory}");
		}

		var excludedExtensions = new List<string> { ".dll", ".exe", ".pdb" };
		var excludedDirectories = new List<string> { "bin", "obj" };

		var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
		var filePaths = Directory.GetFiles(absoluteDirectory, "*.*", searchOption);
		var messageContentToBot = message + "\n\n\n";
		messageContentToBot += "Working directory: " + workingDirectory + "\n\n";

		foreach (var filePath in filePaths)
		{
			var extension = Path.GetExtension(filePath);
			var directoryName = Path.GetDirectoryName(filePath);

			if (excludedExtensions.Contains(extension) || excludedDirectories.Any(dir => filePath.Contains(dir)))
			{
				continue;
			}

			var relativeFilePath = filePath.Replace(workingDirectory, "");

			messageContentToBot += relativeFilePath + "\n";
		}

		// Disabled. This line is only here for debugging, to return the outgoing message before it's sent
		//return Success(messageContentToBot);

		var messageBackendProvider = new OpenAIBackendProvider("https://api.openai.com", apiKey);
		var messageToBot = new Message(messageContentToBot);
		var response = await messageBackendProvider.SendAndReceive(messageToBot);
		if (string.IsNullOrEmpty(response.Content))
		{
			return Fail("No response received from OpenAI API.");
		}
		return Success(response.Content);
	}
}
