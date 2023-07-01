using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using DynamicApi.Contracts;
using DynamicApiServer.Definitions.EndpointDefinitions;
using System.Text.Json;

public class ListEndpointsScriptEndpoint : IDynamicEndpointExecutor
{
    public Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
        var directories = Directory.GetDirectories(parameters.Resolver.WorkingDirectory() + "/" + parameters.ApiConfig.EndpointPath);
        var endpoints = new List<object>();

        foreach (var dir in directories)
        {
            var endpointJsonPath = Path.Combine(dir, "endpoint.json");
            if (File.Exists(endpointJsonPath))
            {
                var endpointJson = File.ReadAllText(endpointJsonPath);
                var endpointInfo = JsonSerializer.Deserialize<EndpointDefinition>(endpointJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                endpoints.Add(new
                {
                    path = endpointInfo.Path,
                    method = endpointInfo.Method
                });
            }
        }

        var json = JsonSerializer.Serialize(endpoints);
        return Task.FromResult(new EndpointExecutionResult
        {
            Body = json
        });
    }
}
