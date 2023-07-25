using System;
using TalkFlow.Messages.Core.Provider;
using TalkFlow.Messages.Core.Factory.Services;
using TalkFlow.Messages.Core.Factory;
using TalkFlow.Messages.Core.Services;
using TalkFlow.Messages.Model;
using TalkFlow.Messages.Core;
using TalkFlow.Messages.Contracts.Factory;
using TalkFlow.Messages.Providers.Backend.OpenAI;
using TalkFlow.Messages.Providers.Backend.OpenAI.Services;
using System.Threading.Tasks;
using DynamicApi.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Abstractions;
using TalkFlow.Chat.Core;
using System.IO;
using TalkFlow.Chat.Core.Factory.Services;
using TalkFlow.Chat.Core.Services;
using TalkFlow.Chat.Providers.Persistence.Json.Services;
using TalkFlow.Messages.Core.Extensions.Services;

public class AiBotSendFilesScriptEndpoint : DynamicEndpointExecutorBase
{

    public override async Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
        var logger = parameters.LoggerFactory.CreateLogger<AiBotSendFilesScriptEndpoint>();
    
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
//         var messageBackendProvider = new OpenAIBackendProvider("https://api.openai.com", apiKey);
//         var messageToBot = new Message(content);
//         var response = await messageBackendProvider.SendAndReceive(messageToBot);
//         
//         return response.Content;


        var serviceCollection = new ServiceCollection()
    .AddSingleton<IFileSystem, System.IO.Abstractions.FileSystem>()
    .AddLogging(config=>
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
        
        IMessageEngine messageEngine = serviceProvider.GetRequiredService<IMessageEngine>();
    
        var messageFactory = serviceProvider.GetRequiredService<IMessageFactory>();
            
        var inputMessage = messageFactory.CreateMessageSent(content);
    
        var responseMessage = await messageEngine.SendAndReceive(inputMessage);

        return responseMessage.Body;
    }
}
