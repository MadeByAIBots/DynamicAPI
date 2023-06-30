using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace DynamicApiServer.Tests.Integration.Endpoints.Files
{
    public class FileLineInsertIntegrationTests
    {
        [Test]
        public async Task TestFileLineInsertEndpoint()
        {
            using var context = new IntegrationTestContext();
            context.UseToken();

            // Set up
            var workingDirectory = Path.GetTempPath();
            var filePath = Path.GetRandomFileName();
            var lines = new[] { "First line", "Second line", "Third line" };
            await File.WriteAllLinesAsync(Path.Combine(workingDirectory, filePath), lines);

            // Exercise
            var response = await context.Client.PostAsync($"/file-line-insert", new StringContent("{ \"workingDirectory\": \"" + workingDirectory + "\", \"filePath\": \"" + filePath + "\", \"before-line-number\": \"3\", \"new-content\": \"New line\" }", Encoding.UTF8, "application/json"));

            // Verify
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Trim().Should().Be("Line inserted successfully");

            var updatedLines = await File.ReadAllLinesAsync(Path.Combine(workingDirectory, filePath));
            updatedLines[2].Should().Be("New line");

            // Teardown
            File.Delete(Path.Combine(workingDirectory, filePath));
        }
    }
}