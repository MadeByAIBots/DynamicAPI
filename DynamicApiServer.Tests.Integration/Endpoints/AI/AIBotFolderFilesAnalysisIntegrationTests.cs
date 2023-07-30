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
    public class AIBotFolderFilesAnalysisIntegrationTests
    {
        [Test]
        public async Task AskTheFilesEndpointTests()
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
                    .GetRequiredService<ILogger<AIBotFolderFilesAnalysisIntegrationTests>>();

                // Set up
                var uniqueId = Guid.NewGuid().ToString();
                var workingDirectory = Path.Combine(Path.GetTempPath(), uniqueId);
                logger.LogInformation("Test setup: Created working directory at {0}", workingDirectory);
                Directory.CreateDirectory(workingDirectory);

                var directoryPath = Path.Combine(workingDirectory, "subdirectory");
                logger.LogInformation("Test setup: Created directory at {0}", directoryPath);
                Directory.CreateDirectory(directoryPath);

                var filePath1 = Path.Combine(workingDirectory, "file.txt");
                logger.LogInformation("Test setup: Created file at {0}", filePath1);
                await File.WriteAllTextAsync(filePath1, "Test content");

                var filePath2 = Path.Combine(workingDirectory, "Program.cs");
                logger.LogInformation("Test setup: Created file at {0}", filePath2);
                await File.WriteAllTextAsync(filePath2, @"using System;
class Program
{
    static void Main()
    {
        Console.WriteLine(""Hello, World!"");
        }
    }
    ");

                var filePath3 = Path.Combine(workingDirectory, "ConfigServicesExtensions.cs");
                logger.LogInformation("Test setup: Created file at {0}", filePath3);
                await File.WriteAllTextAsync(filePath3, @"using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLogging(this IServiceCollection services, Action<ILoggingBuilder> configure)
    {
        return services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders(); // Clear any previously registered providers
            loggingBuilder.AddConsole(); // Add console logging
            configure(loggingBuilder); // Apply additional configuration
        });
    }
}");


                var filePath4 = Path.Combine(workingDirectory, "test.log");
                logger.LogInformation("Test setup: Created file at {0}", filePath4);
                await File.WriteAllTextAsync(filePath4, "Test content");

                var filePath5 = Path.Combine(workingDirectory, "file.dll");
                logger.LogInformation("Test setup: Created file at {0}", filePath5);
                await File.WriteAllTextAsync(filePath5, "Test content");

                var message = "Where can I find the code which configures logging for the C# console application?";

                // TODO: Remove if not needed. This is to test the endpoint on a real code base to ensure it doesn't hit the token limits
                //message = "What class is responsible for handling execution for the csharp script endpoint?";
                //message = "What does this project do?";

                // TODO: Remove if not needed. This is to test the endpoint on a real code base to ensure it doesn't hit the token limits
                //workingDirectory = "/home/madebyaibots/workspace/DynamicAPI";
                // Exercise
                var payload = new { workingDirectory, message }; //, directoryPath };
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8,
                    "application/json");
                logger.LogInformation("Test exercise: Sent POST request to /ai-bot-files-content-analysis");
                var response = await context.Client.PostAsync("/ai-bot-files-content-analysis", content);


                // Verify
                response.StatusCode.Should().Be(HttpStatusCode.OK);
                var responseContent = await response.Content.ReadAsStringAsync();
                logger.LogInformation(responseContent);
                logger.LogInformation("Test verify: Response content is \n{0}", responseContent);
                responseContent.Should().Contain("ServicesExtensions.cs");
                //responseContent.Should().NotContain("Program.cs");
                responseContent.Should().NotContain("file.dll");

                // Teardown
                //Directory.Delete(workingDirectory, true);
            }
        }
    }
}
