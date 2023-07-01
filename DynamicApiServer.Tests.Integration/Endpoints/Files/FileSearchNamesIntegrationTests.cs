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
    public class FileSearchNamesIntegrationTests
    {
        [Test]
        public async Task TestFileSearchNamesEndpoint()
        {
            using var context = new IntegrationTestContext();
            context.UseToken();

            // Set up
            var uniqueId = Guid.NewGuid().ToString();
            var workingDirectory = Path.Combine(Path.GetTempPath(), uniqueId);
            Directory.CreateDirectory(workingDirectory);

            var fileName = Path.GetRandomFileName();
            await File.WriteAllTextAsync(Path.Combine(workingDirectory, fileName), "Test content");

            // Exercise
            var payload = new { workingDirectory, fileName };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await context.Client.PostAsync("/file-search-names", content);

            // Verify
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Trim().Should().Be(Path.Combine(workingDirectory, fileName));

            // Teardown
            Directory.Delete(workingDirectory, true);
        }
    }
}