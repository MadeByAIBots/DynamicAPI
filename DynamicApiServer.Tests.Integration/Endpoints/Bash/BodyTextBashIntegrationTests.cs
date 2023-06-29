using FluentAssertions;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;
using DynamicApiServer.Definitions.EndpointDefinitions;
using DynamicApiServer.Definitions.ExecutorDefinitions;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace DynamicApiServer.Tests.Integration
{
    public class BodyTextBashIntegrationTests
    {
        [Test]
        public async Task TestBashHelloTargetBodyTextEndpoint()
        {
            using var context = new IntegrationTestContext();
            context.UseToken();

            var endpointPath = context.Endpoint()
                .Create("bash-hello-target-body-text-test")
                .AddFile(new EndpointDefinition
                {
                    Path = "/bash-hello-target-body-text-test",
                    Executor = "bash",
                    Method = "post",
                    Args = new List<EndpointArgumentDefinition>
                    {
                        new EndpointArgumentDefinition
                        {
                            Name = "target",
                            Type = "string",
                            Source = "body",
                            Description = "The plain text body containing the target to say hello to."
                        }
                    }
                })
                .AddFile(new BashExecutorDefinition
                {
                    Command = "echo Hello, ${target}!"
                })
                .GetEndpointPath();

            var textBody = "world";

            var response = await context.Client.PostAsync("/" + endpointPath, new StringContent(textBody, Encoding.UTF8, "text/plain"));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Trim().Should().Be("Hello, world!");
        }
    }
}