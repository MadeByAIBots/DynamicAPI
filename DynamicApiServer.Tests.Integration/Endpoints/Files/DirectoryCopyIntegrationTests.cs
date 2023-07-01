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
    public class DirectoryCopyIntegrationTests
    {
        [Test]
        public async Task TestDirectoryCopyEndpoint()
        {
            using var context = new IntegrationTestContext();
            context.UseToken();

            // Set up
            var uniqueId = Guid.NewGuid().ToString();
            var sourcePath = Path.Combine(Path.GetTempPath(), uniqueId, "source");
            var destinationPath = Path.Combine(Path.GetTempPath(), uniqueId, "destination");
            Directory.CreateDirectory(sourcePath);
            var fileName = Path.GetRandomFileName();
            await File.WriteAllTextAsync(Path.Combine(sourcePath, fileName), "Test content");

            // Exercise
            var payload = new { sourcePath, destinationPath };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await context.Client.PostAsync("/directory-copy", content);

            // Verify
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            File.Exists(Path.Combine(destinationPath, fileName)).Should().BeTrue();

            // Teardown
            Directory.Delete(sourcePath, true);
            Directory.Delete(destinationPath, true);
        }
    }
}