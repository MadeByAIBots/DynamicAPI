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
    public class FileReadLinesIntegrationTests
    {
        [Test]
        public async Task TestFileReadLinesEndpoint()
        {
            using var context = new IntegrationTestContext();
            context.UseToken();

            // Set up
            var workingDirectory = Path.GetTempPath();
            var filePath = Path.GetRandomFileName();
            var lines = new[] { "First line", "Second line", "Third line" };
            await File.WriteAllLinesAsync(Path.Combine(workingDirectory, filePath), lines);

            // Exercise
            var response = await context.Client.GetAsync($"/file-read-lines?workingDirectory={workingDirectory}&file-path={filePath}");

            // Verify
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Trim().Should().Be("1: First line\n2: Second line\n3: Third line");

            // Teardown
            File.Delete(Path.Combine(workingDirectory, filePath));
        }
    }
}