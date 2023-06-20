using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;

public class EndpointExecutor
{
    public void CreateEndpoints(WebApplication app, List<EndpointConfiguration> endpointConfigurations)
    {
        Console.WriteLine($"Creating {endpointConfigurations.Count} endpoints...");

        foreach (var config in endpointConfigurations)
        {
            Console.WriteLine($"Creating endpoint: Path = {config.Path}, Executor = {config.Executor}, Command = {config.Command}");

            if (config.Executor == "bash")
            {
                try
                {
                    app.MapGet(config.Path, () =>
                    {
                        Console.WriteLine($"Endpoint {config.Path} called.");

                        var process = new Process()
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "/bin/bash",
                                Arguments = $"-c \"{config.Command}\"",
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                UseShellExecute = false,
                                CreateNoWindow = true,
                            }
                        };

                        process.Start();

                        string result = process.StandardOutput.ReadToEnd();
                        string error = process.StandardError.ReadToEnd();

                        process.WaitForExit();

                        if (!string.IsNullOrEmpty(error))
                        {
                            Console.WriteLine($"Error executing command: {error}");
                        }

                        Console.WriteLine($"Command output: {result}");

                        return result;
                    });

                    Console.WriteLine($"Endpoint {config.Path} created successfully.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error creating endpoint: {e.Message}");
                }
            }
        }
    }
}