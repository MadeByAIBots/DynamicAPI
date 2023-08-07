using System.Diagnostics;
using System.IO.Abstractions;
using System.Text;
using System.Text.Json;
using DynamicApi.Contracts;
using DynamicApi.Endpoints.Model;
using DynamicApi.Utilities.Files;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TalkFlow.Messages;
using TalkFlow.Messages;
using TalkFlow.Messages.Extensions.Services;
using TalkFlow.Messages.Factory.Services;
using TalkFlow.Messages.Services;
using TalkFlow.Messages.Model;
using TalkFlow.Messages.Providers.Backend.OpenAI.Services;

namespace AIBotMessageEndpoint;

public class AIBotMessageEndpoint : DynamicEndpointExecutorBase
{
	public override async Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
	{
		if (!parameters.ApiConfig.Variables.TryGetValue("OpenAIKey", out var apiKey))
		{
			return Fail("OpenAI API key not found in configuration.");
		}

		var messageContent = parameters.GetRequiredString("message");

		var response = await SendAndReceive(messageContent);
		
		return Success(response);
	}
	
	public async Task<string> SendAndReceive(string content)
	{
		var serviceCollection = new ServiceCollection()
			.AddSingleton<IFileSystem, System.IO.Abstractions.FileSystem>()
			.AddLogging(config =>
			{
				config.AddConsole();
				config.SetMinimumLevel(LogLevel.Debug);
			})
			.AddMessageEngine()
			.AddMessageFactory()
			.AddMessageExtensions()
			.AddOpenAIBackendProvider();

		var serviceProvider = serviceCollection.BuildServiceProvider();

		var messageEngine = serviceProvider.GetRequiredService<IMessageEngine>();
		var response = await messageEngine.SendAndReceive(new MessageSent(content));

		return response.Body;
	}
}
