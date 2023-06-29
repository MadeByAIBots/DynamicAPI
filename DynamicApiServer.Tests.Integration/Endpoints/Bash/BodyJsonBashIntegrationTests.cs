using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace DynamicApiServer.Tests.Integration.Endpoints.Bash
{
    public class BodyJsonBashIntegrationTests
    {
        [Test]
        public async Task TestBashHelloTargetBodyJsonEndpoint()
        {
            using var context = new IntegrationTestContext();
            context.UseToken();

            // Construct JSON body
            var jsonBody = "{\"target\": \"world\"}";

            // Send a POST request to the endpoint with JSON body
            var response = await context.Client.PostAsync("/bash-hello-target-body-json", new StringContent(jsonBody, Encoding.UTF8, "application/json"));

            // Assert that the response is correct
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Trim().Should().Be("Hello, world!");
        }
    }
}