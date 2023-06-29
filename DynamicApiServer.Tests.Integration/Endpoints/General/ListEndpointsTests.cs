using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace DynamicApiServer.Tests.Integration.Endpoints.General
{
    public class ListEndpointsTests
    {
        [Test]
        public async Task TestListEndpoints()
        {
            using var context = new IntegrationTestContext();

            // Send a request to the endpoint
            var response = await context.Client.GetAsync("/endpoint-list");

            // Read the response content
            var content = await response.Content.ReadAsStringAsync();

            // Assert that the response is correct
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Contain("bash-hello-world");
            content.Should().Contain("bash-hello-target");
            content.Should().Contain("bash-hello-target-body-text");
        }
    }
}