using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace DynamicApiServer.Tests.Integration.Endpoints.General
{
    public class RunPythonCodeEndpointTests
    {
        [Test]
        public async Task TestRunPythonCodeEndpoint()
        {
            using var context = new IntegrationTestContext();
            context.UseToken();

            var resolver = context.Server.Services.GetRequiredService<WorkingDirectoryResolver>();
            var workingDirectory = resolver.WorkingDirectory();

            // Create the request content
            var content = new StringContent(
                "{\"pythonCode\": \"print('hello')\", \"workingDirectory\": \"" + workingDirectory + "\"}",
                Encoding.UTF8,
                "application/json"
            );

            // Send a request to the endpoint
            var response = await context.Client.PostAsync("/run-python-code", content);

            // Read the response content
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert that the response is correct
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.Trim().Should().Be("hello");
        }
    }
}