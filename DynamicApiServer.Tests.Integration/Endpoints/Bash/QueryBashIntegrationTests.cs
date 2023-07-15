using FluentAssertions;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;
using DynamicApi.Endpoints.Model;
using DynamicApi.Endpoints.Executors.Model;
using System.Collections.Generic;

namespace DynamicApiServer.Tests.Integration
{
    public class QueryBashIntegrationTests
    {
        [Test]
        [TestCase("universe")]
        [TestCase("world")]
        [TestCase("OpenAI")]
        public async Task TestBashHelloTargetEndpoint(string target)
        {
            using var context = new IntegrationTestContext();
            context.UseToken();

            var endpointPath = context.Endpoint()
                .Create($"bash-hello-target-test-{target}")
                .AddFile(new EndpointDefinition
                {
                    Path = $"/bash-hello-target-test-{target}",
                    Executor = "bash",
                    Method = "get",
                    Args = new List<EndpointArgumentDefinition>
                    {
                        new EndpointArgumentDefinition
                        {
                            Name = "target",
                            Type = "string",
                            Source = "query",
                            Description = $"The target to say hello to. Test case: {target}"
                        }
                    }
                })
                .AddFile(new BashExecutorDefinition
                {
                    Command = "echo Hello, ${target}!"
                })
                .GetEndpointPath();

            var response = await context.Client.GetAsync($"/{endpointPath}?target={target}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Trim().Should().Be($"Hello, {target}!");
            //response.Content.Headers.ContentType.MediaType.Should().Be("text/plain");
        }

        [Test]
        public async Task TestBashHelloTargetEndpointWithoutTarget()
        {
            using var context = new IntegrationTestContext();
            context.UseToken();

            var endpointPath = context.Endpoint()
                .Create("bash-hello-target-test-error")
                .AddFile(new EndpointDefinition
                {
                    Path = "/bash-hello-target-test-error",
                    Executor = "bash",
                    Method = "get",
                    Args = new List<EndpointArgumentDefinition>
                    {
                        new EndpointArgumentDefinition
                        {
                            Name = "target",
                            Type = "string",
                            Source = "query",
                            Description = "The target to say hello to."
                        }
                    }
                })
                .AddFile(new BashExecutorDefinition
                {
                    Command = "echo Hello, ${target}!"
                })
                .GetEndpointPath();

            var response = await context.Client.GetAsync($"/{endpointPath}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}