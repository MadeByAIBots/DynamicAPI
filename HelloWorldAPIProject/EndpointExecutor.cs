using HelloWorldAPIProject.Definitions.EndpointDefinitions;
using HelloWorldAPIProject.Definitions.ExecutorDefinitions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;

public class EndpointExecutor
{
    private readonly BashExecutorConfiguration _executorConfig;

    public EndpointExecutor(BashExecutorConfiguration executorConfig)
    {
        _executorConfig = executorConfig;
        Console.WriteLine($"EndpointExecutor initialized with executorConfig: {_executorConfig}");
    }

    public void Execute(IEndpointRouteBuilder endpoints, EndpointConfiguration config)
    {
        endpoints.MapPost(config.Path, async context =>
        {
            var process = new System.Diagnostics.Process()
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "bash",
                    Arguments = $"-c \"{_executorConfig.Command}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string result = await process.StandardOutput.ReadToEndAsync();
            process.WaitForExit();
            await context.Response.WriteAsync(result);
        });
    }
}