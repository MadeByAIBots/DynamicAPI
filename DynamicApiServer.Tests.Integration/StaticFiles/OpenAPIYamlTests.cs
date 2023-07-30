using System.Configuration;
using FluentAssertions;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;

namespace DynamicApiServer.Tests.Integration.StaticFiles
{
    public class OpenAPIYamlTests : IntegrationTestContext
    {
        
        [Test]
        public async Task TestServerUrlFound()
        {
            using var context = new IntegrationTestContext();

            var config = context.Server.Services.GetRequiredService<ApiConfiguration>();

            var random = new Random();
            var randomServerUrl = $"http://domain{random.Next()}.com/";

            config.ExternalUrl = randomServerUrl;

            // Send a request to the endpoint
            var response = await context.Client.GetAsync("/openapi.yaml");

            // Assert that the response is correct
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain(randomServerUrl);

        }
    }
}
