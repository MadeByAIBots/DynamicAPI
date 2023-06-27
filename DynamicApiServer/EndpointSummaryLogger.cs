using DynamicApiServer.Definitions.EndpointDefinitions;
using System;
using System.Collections.Generic;

public class EndpointSummaryLogger
{
    public void LogSummary(List<EndpointConfiguration> endpointConfigs)
    {
        // Log the count of endpoints
        Console.WriteLine($"Total endpoints loaded and mapped: {endpointConfigs.Count}");

        // Log the details of each endpoint
        foreach (var config in endpointConfigs)
        {
            Console.WriteLine($"Endpoint: {config.Path}, Executor: {config.Executor}");
        }
    }
}