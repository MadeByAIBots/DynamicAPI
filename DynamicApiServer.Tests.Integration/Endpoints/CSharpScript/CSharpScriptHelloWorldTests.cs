using FluentAssertions;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;
using DynamicApiServer.Definitions.EndpointDefinitions;
using DynamicApiServer.Definitions.ExecutorDefinitions;
using System.Net.Http;

namespace DynamicApiServer.Tests.Integration
{
    public class CSharpScriptHelloWorldTests
    {
        [Test]
        public async Task TestCSharpHelloWorldEndpoint()
        {
            using var context = new IntegrationTestContext();
            context.UseToken();

            var endpointPath = context.Endpoint()
                .Create("csharp-script-hello-world-test")
                .AddFile(new EndpointDefinition
                {
                    Path = "/csharp-script-hello-world-test",
                    Executor = "csharp-script",
                    Method = "get"
                })
                .AddFile(new CSharpScriptExecutorDefinition
                {
                    Script = "HelloWorldScript.csx"
                })
                .AddFile("HelloWorldScript.csx", "using System;\nusing System.Collections.Generic;\nusing System.Threading.Tasks;\nusing DynamicApi.Contracts;\n\npublic class HelloWorldScriptEndpoint : IDynamicEndpointExecutor\n{\n    public Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)\n    {\n        return Task.FromResult(new EndpointExecutionResult\n        {\n            Body = \"Hello, World!\"\n        });\n    }\n}\n")
                .GetEndpointPath();

            var response = await context.Client.GetAsync("/" + endpointPath);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Trim().Should().Be("Hello, World!");
        }
    }
}