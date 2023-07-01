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
    public class FileMoveIntegrationTests
    {
        [Test]
        public async Task TestFileMoveEndpoint()
        {
            using var context = new IntegrationTestContext();
            context.UseToken();

            // Set up
            var uniqueId = Guid.NewGuid().ToString();
            var sourcePath = Path.Combine(Path.GetTempPath(), uniqueId, "source.txt");
            var destinationPath = Path.Combine(Path.GetTempPath(), uniqueId, "destination.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(sourcePath));
            await File.WriteAllTextAsync(sourcePath, "Test content");

            // Exercise
            var payload = new { sourcePath, destinationPath };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await context.Client.PostAsync("/file-move", content);

            // Verify
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            File.Exists(destinationPath).Should().BeTrue();

            // Teardown
            File.Delete(destinationPath);
        }
    }
}