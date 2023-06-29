using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace DynamicApiServer.Tests.Integration.Endpoints.Bash
{
    public class BodyTextBashIntegrationTests
    {
        [Test]
        public async Task TestBashHelloTargetBodyTextEndpoint()
        {
            using var context = new IntegrationTestContext();
            context.UseToken();

            // Construct text body
            var textBody = "world";

            // Send a POST request to the endpoint with text body
            var response = await context.Client.PostAsync("/bash-hello-target-body-text", new StringContent(textBody, Encoding.UTF8, "text/plain"));

            // Assert that the response is correct
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Trim().Should().Be("Hello, world!");
        }
    }
}