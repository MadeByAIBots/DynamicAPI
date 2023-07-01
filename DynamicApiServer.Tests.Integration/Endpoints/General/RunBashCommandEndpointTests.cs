using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace DynamicApiServer.Tests.Integration.Endpoints.General
{
    public class ExecuteEndpointTests
    {
        [Test]
        public async Task TestExecuteEndpoint()
        {
            using var context = new IntegrationTestContext();
            context.UseToken();

            // Create the request content
            var content = new StringContent(
                "{\"command\": \"echo hello\", \"workingDirectory\": \"/root\"}",
                Encoding.UTF8,
                "application/json"
            );

            // Send a request to the endpoint
            var response = await context.Client.PostAsync("/run-bash-command", content);

            // Read the response content
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert that the response is correct
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.Trim().Should().Be("hello");
        }
    }
}