using FluentAssertions;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;

namespace DynamicApiServer.Tests.Integration.Auth
{
    public class AuthenticationTests : IntegrationTestContext
    {
        
        [Test]
        public async Task TestUnauthorisedAccess()
        {
            using var context = new IntegrationTestContext();

            // Send a request to the endpoint
            var response = await context.Client.GetAsync("/endpoint-list");

            // Assert that the response is correct
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
