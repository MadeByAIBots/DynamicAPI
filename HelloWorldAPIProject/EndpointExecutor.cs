using HelloWorldAPIProject.Definitions.EndpointDefinitions;
using HelloWorldAPIProject.Definitions.ExecutorDefinitions;
using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;

public class EndpointExecutor
{
    public void CreateEndpoints(WebApplication app, List<EndpointConfiguration> configurations, EndpointLoader endpointLoader)
    {
        foreach (var config in configurations)
        {
            Console.WriteLine($"Creating endpoint: Path = {config.Path}, Executor = {config.Executor}");

            if (config.Executor == "bash")
            {
                var executorConfiguration = endpointLoader.GetExecutorConfiguration(config.Executor);

                app.MapGet(config.Path, () =>
                {
                    var process = new Process()
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "/bin/bash",
                            Arguments = "-c \"" + executorConfiguration.Command + "\"",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                        }
                    };
                    process.Start();
                    string result = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return result;
                });

                Console.WriteLine($"Endpoint {config.Path} created successfully.");
            }
            else
            {
                Console.WriteLine($"Unsupported executor: {config.Executor}");
            }
        }
    }
}