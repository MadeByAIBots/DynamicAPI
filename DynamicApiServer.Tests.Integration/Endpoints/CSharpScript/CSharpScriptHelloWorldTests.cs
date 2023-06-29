using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace DynamicApiServer.Tests.Integration.Endpoints.CSharpScript
{
    public class CSharpScriptHelloWorldTests
    {
        [Test]
        public async Task TestHelloWorldEndpoint()
        {
            using var context = new IntegrationTestContext();
            context.UseToken();

            // Send a request to the endpoint
            var response = await context.Client.GetAsync("/csharp-script-hello-world");

            // Read the response content
            var content = await response.Content.ReadAsStringAsync();

            // Assert that the response is correct
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Contain("Hello, World!");
        }
    }
}