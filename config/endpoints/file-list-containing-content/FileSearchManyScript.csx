#r "/root/workspace/DynamicAPI/DynamicApi.Contracts/bin/Debug/net7.0/DynamicApi.Contracts.dll"
#r "/root/workspace/DynamicAPI/Definitions/EndpointDefinitions/bin/Debug/net7.0/EndpointDefinitions.dll"
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using DynamicApi.Contracts;
using DynamicApiServer.Definitions.EndpointDefinitions;

public class FileSearchManyScriptEndpoint : IDynamicEndpointExecutor
{
    public async Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
        var workingDirectory = parameters.Parameters["workingDirectory"];
        var query = parameters.Parameters["query"];
        var searchInContent = bool.Parse(parameters.Parameters["searchInContent"]);
        var searchInNames = bool.Parse(parameters.Parameters["searchInNames"]);
        var recursive = bool.Parse(parameters.Parameters["recursive"]);

        string recursiveFlag = recursive ? "-r" : "";
        string grepCommand = searchInContent ? $"grep {recursiveFlag} -l '{query}' ." : "";
        string findCommand = searchInNames ? $"find . {recursiveFlag} -name '*{query}*'" : "";

        string command = "";
        if (searchInContent) command += grepCommand;
        if (searchInNames) command += (command.Length > 0 ? "; " : "") + findCommand;

        var processStartInfo = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"{command}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workingDirectory
        };

        var process = new Process
        {
            StartInfo = processStartInfo
        };

        var outputBuilder = new StringBuilder();

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

        string result = outputBuilder.ToString();

        return new EndpointExecutionResult
        {
            Body = result
        };
    }
}