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
    public class DirectoryCreateIntegrationTests
    {
        [Test]
        public async Task TestDirectoryCreateEndpoint()
        {
            using var context = new IntegrationTestContext();
            context.UseToken();

            // Set up
            var uniqueId = Guid.NewGuid().ToString();
            var workingDirectory = Path.Combine(Path.GetTempPath(), uniqueId);
            var directoryName = Guid.NewGuid().ToString();

            // Exercise
            var payload = new { workingDirectory, directoryName };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await context.Client.PostAsync("/directory-create", content);

            // Verify
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Directory.Exists(Path.Combine(workingDirectory, directoryName)).Should().BeTrue();

            // Teardown
            Directory.Delete(workingDirectory, true);
        }
    }
}