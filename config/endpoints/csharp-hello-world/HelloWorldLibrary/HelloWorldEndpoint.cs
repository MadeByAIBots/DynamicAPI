using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DynamicApi.Contracts;

namespace HelloWorldLibrary
{
    public class HelloWorldEndpoint : IDynamicEndpointExecutor
    {
        public Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
        {
            return Task.FromResult(new EndpointExecutionResult
            {
                Body = "Hello, World!"
            });
        }
    }
}