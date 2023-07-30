using System.Configuration;
using FluentAssertions;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;

namespace DynamicApiServer.Tests.Integration.StaticFiles
{
    public class PluginJsonTests : IntegrationTestContext
    {
        
        [Test]
        public async Task TestOpenAIVerificationTokenFound()
        {
            using var context = new IntegrationTestContext();

            var config = context.Server.Services.GetRequiredService<ApiConfiguration>();

            var randomOpenAIVerificationToken = Guid.NewGuid().ToString();

            config.OpenAIVerificationToken = randomOpenAIVerificationToken;

            // Send a request to the endpoint
            var response = await context.Client.GetAsync("/.well-known/ai-plugin.json");

            // Assert that the response is correct
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain(randomOpenAIVerificationToken);

        }
    }
}
