using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicApi.Contracts;

public class RunBashCommandScriptEndpoint : DynamicEndpointExecutorBase
{
    private const int DefaultMaxLines = 10;

    public override async Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
        try
        {
            var command = parameters.GetRequiredString("command");
            var workingDirectory = parameters.GetRequiredString("workingDirectory");
            var maxLines = parameters.GetInt32("maxLines");
            if (maxLines == 0)
            {
                maxLines = DefaultMaxLines;
            }

            var result = await RunBashCommandAsync(command, workingDirectory);

            // Limit the number of output lines if maxLines is set
            var outputLines = result.CombinedOutput.Split('\n');
            var limitedOutput = string.Join('\n', outputLines.Reverse().Take(maxLines).Reverse());
            
            if (result.ExitCode != 0)
            {
                return Fail($"Command execution failed with exit code {result.ExitCode}. Output: {limitedOutput}");
            }

            return Success(limitedOutput);
        }
        catch (Exception ex)
        {
            return Fail($"Failed to execute command. Error: {ex.Message}");
        }
    }

    private async Task<(int ExitCode, string CombinedOutput)> RunBashCommandAsync(string command, string workingDirectory)
    {
        var outputBuilder = new StringBuilder();

        using (var process = new Process())
        {
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                ArgumentList = { "-c", command },
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    outputBuilder.AppendLine(e.Data);
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    outputBuilder.AppendLine(e.Data);
                }
            };

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();

            return (process.ExitCode, outputBuilder.ToString());
        }
    }
}