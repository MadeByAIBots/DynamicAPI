using FluentAssertions;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;

namespace DynamicApiServer.Tests.Integration
{
    public class SimpleCSharpIntegrationTests
    {
        [Test]
        public async Task TestCSharpHelloWorldEndpoint()
        {
            using var context = new IntegrationTestContext();
            context.UseToken();

            // Send a request to the endpoint
            var response = await context.Client.GetAsync("/csharp-hello-world");

            // Assert that the response is correct
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Trim().Should().Be("Hello, World!");
        }

        // Add more tests here, following the same pattern
    }
}