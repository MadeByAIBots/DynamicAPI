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

            // Set up
            var uniqueId = Guid.NewGuid().ToString();
            var workingDirectory = Path.Combine(Path.GetTempPath(), uniqueId);
            Directory.CreateDirectory(workingDirectory);

            var subdirectoryPath = Path.Combine(workingDirectory, "subdirectory");
            Directory.CreateDirectory(subdirectoryPath);

            var filePath = Path.Combine(workingDirectory, "file.txt");
            await File.WriteAllTextAsync(filePath, "Test content");

            // Exercise
            var payload = new { workingDirectory };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await context.Client.PostAsync("/directory-list", content);

            // Verify
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().Contain("subdirectory");
            responseContent.Should().Contain("file.txt");

            // Teardown
            Directory.Delete(workingDirectory, true);
        }
    }
}