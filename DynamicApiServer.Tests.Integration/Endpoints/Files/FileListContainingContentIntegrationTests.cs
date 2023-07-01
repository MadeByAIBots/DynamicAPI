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
    public class FileSearchContentsIntegrationTests
    {
        [Test]
        public async Task TestFileSearchContentsEndpoint()
        {
            using var context = new IntegrationTestContext();
            context.UseToken();

            // Set up
            var uniqueId = Guid.NewGuid().ToString();
            var workingDirectory = Path.Combine(Path.GetTempPath(), uniqueId);
            Directory.CreateDirectory(workingDirectory);

            var filePath = Path.GetRandomFileName();
            var lines = new[] { "First line", "Second line", "Third line" };
            await File.WriteAllLinesAsync(Path.Combine(workingDirectory, filePath), lines);

            // Exercise
            var payload = new { workingDirectory, query = "line", recursive = "true" };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await context.Client.PostAsync("/file-list-containing-content", content);

            // Verify
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Trim().Should().Be(Path.Combine(workingDirectory, filePath));

            // Teardown
            Directory.Delete(workingDirectory, true);
        }
    }
}