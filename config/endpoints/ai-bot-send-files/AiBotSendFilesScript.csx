using System;
using TalkFlow.Messages.Core.Provider;
using TalkFlow.Messages.Model;
using TalkFlow.Messages.Providers.Backend.OpenAI;
using System.IO;
using System.Threading.Tasks;
using DynamicApi.Contracts;

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
        var messageContent = "";

        foreach (var path in filePaths)
        {
            var absolutePath = Path.Combine(workingDirectory, path);

            if (!File.Exists(absolutePath))
            {
                return Fail($"File does not exist at path: {absolutePath}");
            }

            try
            {
                messageContent += path + "\n\n\n" + File.ReadAllText(absolutePath).Trim() + "\n\n\n";
            }
            catch (Exception e)
            {
                return Fail($"Failed to read file at {absolutePath}: {e.Message}");
            }
        }

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
