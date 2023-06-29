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

            // Exercise
            var response = await context.Client.PostAsync($"/file-create?working-directory={workingDirectory}&file-path={filePath}", new StringContent(content, Encoding.UTF8, "text/plain"));

            // Verify
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Trim().Should().Be("File created successfully.");

            var createdFilePath = Path.Combine(workingDirectory, filePath);
            File.Exists(createdFilePath).Should().BeTrue();
            var createdFileContent = await File.ReadAllTextAsync(createdFilePath);
            createdFileContent.Should().Be(content);

            // Teardown
            File.Delete(createdFilePath);
        }
    }
}