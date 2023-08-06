using System.Diagnostics;
using System.Text;
using System.Text.Json;
using DynamicApi.Contracts;
using DynamicApi.Endpoints.Model;
using DynamicApi.Utilities.Files;

namespace EndpointListEndpoint;

public class EndpointListEndpoint : DynamicEndpointExecutorBase
{
	public override async Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
	{
		// TODO: Make this use the endpoint loader
		
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

		return Success(endpoints);
	}
}