using FluentAssertions;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;

namespace DynamicApiServer.Tests.Integration
{
    public class QueryBashIntegrationTests
    {
        [Test]
        public async Task TestBashHelloTargetEndpoint()
        {
            using var context = new IntegrationTestContext();

            // Send a request to the endpoint
            var response = await context.Client.GetAsync("/bash-hello-target?target=universe");

            // Assert that the response is correct
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Trim().Should().Be("Hello, universe!");
        }

        // Add more tests here, following the same pattern
    }
}