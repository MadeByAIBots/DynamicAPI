using System.Diagnostics;
using System.IO.Abstractions;
using System.Text;
using System.Text.Json;
using DynamicApi.Contracts;
using DynamicApi.Endpoints.Model;
using DynamicApi.Utilities.Files;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TalkFlow.Chat;
using TalkFlow.Chat.Factory.Services;
using TalkFlow.Chat.Providers.Persistence.Json.Services;
using TalkFlow.Chat.Services;
using TalkFlow.Messages.Extensions.Services;
using TalkFlow.Messages.Factory.Services;
using TalkFlow.Messages.Services;
using TalkFlow.Messages.Model;
using TalkFlow.Messages.Providers.Backend.OpenAI.Services;

namespace AIBotChatEndpoint;


public class AIBotChatEndpoint : DynamicEndpointExecutorBase
{

    public override async Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
        var logger = parameters.LoggerFactory.CreateLogger<AIBotChatEndpoint>();

//         if (!parameters.ApiConfig.Variables.TryGetValue("OpenAIKey", out var apiKey))
//         {
//             return Fail("OpenAI API key not found in configuration.");
//         }

        var message = parameters.GetRequiredString("message");

        var response = await SendAndReceive(message);

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
            .AddOpenAIBackendProvider()
            .AddChatEngine()
            .AddConversationFactory()
            .AddChatPersistenceJsonProvider(Directory.GetCurrentDirectory());

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var chatEngine = serviceProvider.GetRequiredService<IChatEngine>();
        var convo = chatEngine.CreateConversation();
        var response = await chatEngine.SendAndReceive(convo, new MessageSent(content));

        return response.Body;
    }
}