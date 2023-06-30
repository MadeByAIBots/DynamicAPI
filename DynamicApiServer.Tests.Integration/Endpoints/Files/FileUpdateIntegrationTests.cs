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
    public class FileUpdateIntegrationTests
    {
        [Test]
        public async Task TestFileUpdateEndpoint()
        {
            using var context = new IntegrationTestContext();
            context.UseToken();

            // Set up
            var workingDirectory = Path.GetTempPath();
            var filePath = Path.GetRandomFileName();
            var initialContent = "Initial content";
            File.WriteAllText(Path.Combine(workingDirectory, filePath), initialContent);
            var newContent = "Updated content";
            var jsonContent = new StringContent($"{{\"workingDirectory\": \"{workingDirectory}\", \"file-path\": \"{filePath}\", \"content\": \"{newContent}\"}}", Encoding.UTF8, "application/json");

            // Exercise
            var response = await context.Client.PostAsync("/file-update", jsonContent);

            // Verify
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Trim().Should().StartWith("File updated successfully at");

            var updatedFilePath = Path.Combine(workingDirectory, filePath);
            File.Exists(updatedFilePath).Should().BeTrue();
            var updatedFileContent = await File.ReadAllTextAsync(updatedFilePath);
            updatedFileContent.Should().Be(newContent);

            // Teardown
            File.Delete(updatedFilePath);
        }
    }
}