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
    public class FileCreateIntegrationTests
    {
        [Test]
        public async Task TestFileCreateEndpoint()
        {
            using var context = new IntegrationTestContext();
            context.UseToken();

            // Set up
            var workingDirectory = Path.GetTempPath();
            var filePath = Path.GetRandomFileName();
            var content = "Hello, world!";
            var jsonContent = new StringContent($"{{\"working-directory\": \"{workingDirectory}\", \"file-path\": \"{filePath}\", \"content\": \"{content}\"}}", Encoding.UTF8, "application/json");

            // Exercise
            var response = await context.Client.PostAsync("/file-create", jsonContent);

            // Verify
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Trim().Should().StartWith("File created successfully at");

            var createdFilePath = Path.Combine(workingDirectory, filePath);
            File.Exists(createdFilePath).Should().BeTrue();
            var createdFileContent = await File.ReadAllTextAsync(createdFilePath);
            createdFileContent.Should().Be(content);

            // Teardown
            File.Delete(createdFilePath);
        }
    }
}