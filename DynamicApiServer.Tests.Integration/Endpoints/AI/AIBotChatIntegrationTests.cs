using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace DynamicApiServer.Tests.Integration.Endpoints.AI
{
    public class AIBotChatIntegrationTests
    {
        [Test]
        public async Task AIBotChatTest()
        {
            bool isOpenAIEnabled = bool.TryParse(Environment.GetEnvironmentVariable("ENABLE_OPENAI"), out bool result) ? result : false;

            if (!isOpenAIEnabled)
            {
                Console.WriteLine("OpenAI is not enabled. Skipping text. Set ENABLE_OPENAI to true to enable.");
            }
            else
            {
                using var context = new IntegrationTestContext();
                context.UseToken();
                var logger = context.Server.Services
                    .GetRequiredService<ILogger<AIBotChatIntegrationTests>>();

                var message = "What is the capital of france?";

                var payload = new { message };
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8,
                    "application/json");
                logger.LogInformation("Test exercise: Sent POST request to /ai-bot-files-content-analysis");
                var response = await context.Client.PostAsync("/ai-bot-chat", content);


                // Verify
                response.StatusCode.Should().Be(HttpStatusCode.OK);
                var responseContent = await response.Content.ReadAsStringAsync();
                logger.LogInformation(responseContent);
                logger.LogInformation("Test verify: Response content is \n{0}", responseContent);
                responseContent.ToLower().Should().Contain("paris");
            }
        }
    }
}
