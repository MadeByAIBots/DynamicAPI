using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;

public class EndpointExecutor
{
    public void CreateEndpoints(WebApplication app, List<EndpointConfiguration> configurations)
    {
        foreach (var config in configurations)
        {
            Console.WriteLine($"Creating endpoint: Path = {config.Path}, Executor = {config.Executor}, Command = {config.Command}");

            if (config.Executor == "bash")
            {
                app.MapGet(config.Path, () =>
                {
                    var process = new Process()
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "/bin/bash",
                            Arguments = "-c \"" + config.Command + "\"",
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