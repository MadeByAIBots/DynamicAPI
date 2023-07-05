using System;
using TalkFlow.Messages.Core.Provider;
using TalkFlow.Messages.Model;
using TalkFlow.Messages.Providers.Backend.OpenAI;
using System.IO;
using System.Threading.Tasks;
using DynamicApi.Contracts;

public class AiBotMessageScriptEndpoint : DynamicEndpointExecutorBase
{
	public override async Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
	{
		if (!parameters.ApiConfig.Variables.TryGetValue("OpenAIKey", out var apiKey))
		{
			return Fail("OpenAI API key not found in configuration.");
		}

		var messageContent = parameters.GetRequiredString("message");

		var messageBackendProvider = new OpenAIBackendProvider("https://api.openai.com", apiKey);
		var message = new Message(messageContent);
		var response = await messageBackendProvider.SendAndReceive(message);
		if (string.IsNullOrEmpty(response.Content))
		{
			return Fail("No response received from OpenAI API.");
		}
		return Success(response.Content);
	}
}
