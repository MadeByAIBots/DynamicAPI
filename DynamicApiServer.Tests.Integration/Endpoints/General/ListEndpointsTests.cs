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
            context.UseToken();

            // Send a request to the endpoint
            var response = await context.Client.GetAsync("/endpoint-list");

            // Read the response content
            var content = await response.Content.ReadAsStringAsync();

            // Assert that the response is correct
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Contain("endpoint-list");
            content.Should().Contain("file-create");
        }
    }
}