#r "/root/workspace/DynamicAPI/DynamicApi.Contracts/bin/Debug/net7.0/DynamicApi.Contracts.dll"

using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using DynamicApi.Contracts;

public class ListEndpointsScriptEndpoint : IDynamicEndpointExecutor
{
    public Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
        var directories = Directory.GetDirectories(parameters.ApiConfig.EndpointPath);
        var endpointNames = new List<string>();
        endpointNames.Add("Available endpoints:");
        foreach (var dir in directories)
        {
            endpointNames.Add(Path.GetFileName(dir));
        }

        return Task.FromResult(new EndpointExecutionResult
        {
            Body = string.Join(", ", endpointNames)
        });
    }
}