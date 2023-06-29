using FluentAssertions;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;
using DynamicApiServer.Definitions.EndpointDefinitions;
using DynamicApiServer.Definitions.ExecutorDefinitions;

namespace DynamicApiServer.Tests.Integration
{
    public class SimpleBashIntegrationTests
    {
        [Test]
        public async Task TestBashHelloWorldEndpoint()
        {
            using var context = new IntegrationTestContext();
            context.UseToken();

            var output = "Hello world!";

            var endpointPath = context.Endpoint()
                .Create("bash-hello-world")
                .AddBashEndpoint($"echo {output}", "get")
                .GetEndpointPath();

            // Send a request to the endpoint
            var response = await context.Client.GetAsync("/" + endpointPath);

            // Assert that the response is correct
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Trim().Should().Be(output);
        }
    }
}