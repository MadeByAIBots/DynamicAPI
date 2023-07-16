using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace DynamicApiServer.Tests.Integration.Endpoints.Files
{
    public class DirectoryListIntegrationTests
    {
        [Test]
        public async Task TestDirectoryListEndpoint()
        {
            using var context = new IntegrationTestContext();
            context.UseToken();
            var logger = context.Server.Services.GetRequiredService<ILogger<DirectoryListIntegrationTests>>();

            // Set up
            var uniqueId = Guid.NewGuid().ToString();
            var workingDirectory = Path.Combine(Path.GetTempPath(), uniqueId);
            logger.LogInformation("Test setup: Created working directory at {0}", workingDirectory);
            Directory.CreateDirectory(workingDirectory);

            var directoryPath = Path.Combine(workingDirectory, "subdirectory");
            logger.LogInformation("Test setup: Created directory at {0}", directoryPath);
            Directory.CreateDirectory(directoryPath);

            var filePath = Path.Combine(workingDirectory, "file.txt");
            logger.LogInformation("Test setup: Created file at {0}", filePath);
            await File.WriteAllTextAsync(filePath, "Test content");

            // Exercise
            var payload = new { workingDirectory };//, directoryPath };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            logger.LogInformation("Test exercise: Sent POST request to /directory-list");
            var response = await context.Client.PostAsync("/directory-list", content);

            // Verify
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await response.Content.ReadAsStringAsync();
            logger.LogInformation("Test verify: Response content is \n{0}", responseContent);
            responseContent.Should().Contain("subdirectory");
            responseContent.Should().Contain("file.txt");

            // Teardown
            Directory.Delete(workingDirectory, true);
        }
    }
}
