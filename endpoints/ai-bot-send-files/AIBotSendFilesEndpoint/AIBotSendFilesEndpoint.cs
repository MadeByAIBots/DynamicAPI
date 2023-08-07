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
using TalkFlow.Chat.Services;
using TalkFlow.Messages;
using TalkFlow.Messages.Extensions.Services;
using TalkFlow.Messages.Factory.Services;
using TalkFlow.Messages.Services;
using TalkFlow.Messages.Model;
using TalkFlow.Messages.Providers.Backend.OpenAI.Services;

namespace AIBotSendFilesEndpoint;

public class AiBotSendFilesScriptEndpoint : DynamicEndpointExecutorBase
{
    public override async Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
        if (!parameters.ApiConfig.Variables.TryGetValue("OpenAIKey", out var apiKey))
        {
            return Fail("OpenAI API key not found in configuration.");
        }

        var workingDirectory = parameters.GetRequiredString("workingDirectory");
        var filePaths = parameters.GetRequiredString("files").Split(';');
        var fullMessageToBot = "";

        var message = parameters.GetRequiredString("message");
        fullMessageToBot += message + "\n\n\n";
            
        foreach (var path in filePaths)
        {
            var absolutePath = Path.Combine(workingDirectory, path);

            if (!File.Exists(absolutePath))
            {
                return Fail($"File does not exist at path: {absolutePath}");
            }

            try
            {
                fullMessageToBot += absolutePath + "\n" + File.ReadAllText(absolutePath).Trim() + "\n\n\n";
            }
            catch (Exception e)
            {
                return Fail($"Failed to read file at {absolutePath}: {e.Message}");
            }
        }

        var response = await SendAndReceive(fullMessageToBot);
        
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
