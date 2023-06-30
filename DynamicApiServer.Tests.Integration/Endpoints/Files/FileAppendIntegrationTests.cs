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
    public class FileAppendIntegrationTests
    {
        [Test]
        public async Task TestFileAppendEndpoint()
        {
            using var context = new IntegrationTestContext();
            context.UseToken();

            // Set up
            var workingDirectory = Path.GetTempPath();
            var filePath = Path.GetRandomFileName();
            var initialContent = "Initial content";
            await File.WriteAllTextAsync(Path.Combine(workingDirectory, filePath), initialContent);

            // Exercise
            var contentToAppend = "Appended content";
            var response = await context.Client.PostAsync($"/file-append", new StringContent("{ \"workingDirectory\": \"" + workingDirectory + "\", \"file-path\": \"" + filePath + "\", \"content\": \"" + contentToAppend + "\", \"add-newline\": \"false\" }", Encoding.UTF8, "application/json"));

            // Verify
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Trim().Should().Be("Content appended successfully");

            var updatedContent = await File.ReadAllTextAsync(Path.Combine(workingDirectory, filePath));
            updatedContent.Should().Be(initialContent + contentToAppend);

            // Teardown
            File.Delete(Path.Combine(workingDirectory, filePath));
        }
    }
}