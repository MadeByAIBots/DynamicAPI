#r "/root/workspace/DynamicAPI/DynamicApi.Contracts/bin/Debug/net7.0/DynamicApi.Contracts.dll"

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DynamicApi.Contracts;

public class HelloWorldScriptEndpoint : IDynamicEndpointExecutor
{
    public Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
        return Task.FromResult(new EndpointExecutionResult
        {
            Body = "Hello, World!"
        });
    }
}