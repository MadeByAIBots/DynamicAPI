using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace DynamicApiServer.Tests.Integration.Endpoints.General
{
    public class EndpointNotFoundTests
    {
        [Test]
        public async Task TestEndpointNotFound()
        {
            using var context = new IntegrationTestContext();

            // Send a request to the endpoint
            var response = await context.Client.GetAsync("/not-found");

            // Assert that the response is correct
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}