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
    public class FilePrependIntegrationTests
    {
        [Test]
        public async Task TestFilePrependEndpoint()
        {
            using var context = new IntegrationTestContext();
            context.UseToken();

            // Set up
            var workingDirectory = Path.GetTempPath();
            var filePath = Path.GetRandomFileName();
            var initialContent = "Initial content";
            await File.WriteAllTextAsync(Path.Combine(workingDirectory, filePath), initialContent);

            // Exercise
            var contentToPrepend = "Prepended content";
            var response = await context.Client.PostAsync($"/file-prepend", new StringContent("{ \"workingDirectory\": \"" + workingDirectory + "\", \"filePath\": \"" + filePath + "\", \"content\": \"" + contentToPrepend + "\", \"addNewline\": \"false\" }", Encoding.UTF8, "application/json"));

            // Verify
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Trim().Should().Be("Content prepended successfully");

            var updatedContent = await File.ReadAllTextAsync(Path.Combine(workingDirectory, filePath));
            updatedContent.Should().Be(contentToPrepend + initialContent);

            // Teardown
            File.Delete(Path.Combine(workingDirectory, filePath));
        }
    }
}