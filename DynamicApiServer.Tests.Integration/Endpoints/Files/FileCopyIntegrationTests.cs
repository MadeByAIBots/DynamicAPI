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
    public class FileCopyIntegrationTests
    {
        [Test]
        public async Task TestFileCopyEndpoint()
        {
            using var context = new IntegrationTestContext();
            context.UseToken();

            // Set up
            var uniqueId = Guid.NewGuid().ToString();
            var sourcePath = Path.Combine(Path.GetTempPath(), uniqueId, "source.txt");
            var destinationPath = Path.Combine(Path.GetTempPath(), uniqueId, "destination.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(sourcePath));
            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
            await File.WriteAllTextAsync(sourcePath, "Test content");

            // Exercise
            var payload = new { sourcePath, destinationPath };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await context.Client.PostAsync("/file-copy", content);

            // Verify
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            File.Exists(destinationPath).Should().BeTrue();

            // Teardown
            File.Delete(sourcePath);
            File.Delete(destinationPath);
        }
    }
}