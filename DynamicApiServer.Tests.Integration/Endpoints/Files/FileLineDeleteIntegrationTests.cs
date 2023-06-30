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
    public class FileLineDeleteIntegrationTests
    {
        [Test]
        public async Task TestFileLineDeleteEndpoint()
        {
            using var context = new IntegrationTestContext();
            context.UseToken();

            // Set up
            var workingDirectory = Path.GetTempPath();
            var filePath = Path.GetRandomFileName();
            var lines = new[] { "First line", "Second line", "Third line" };
            await File.WriteAllLinesAsync(Path.Combine(workingDirectory, filePath), lines);

            // Exercise
            var response = await context.Client.PostAsync($"/file-line-delete", new StringContent("{ \"working-directory\": \"" + workingDirectory + "\", \"file-path\": \"" + filePath + "\", \"line-number\": \"2\" }", Encoding.UTF8, "application/json"));

            // Verify
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Trim().Should().Be("Line deleted successfully");

            var updatedLines = await File.ReadAllLinesAsync(Path.Combine(workingDirectory, filePath));
            updatedLines.Length.Should().Be(2);
            updatedLines[0].Should().Be("First line");
            updatedLines[1].Should().Be("Third line");

            // Teardown
            File.Delete(Path.Combine(workingDirectory, filePath));
        }
    }
}