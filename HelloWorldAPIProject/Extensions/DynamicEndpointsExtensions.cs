using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HelloWorldAPIProject.Extensions
{
    public static class DynamicEndpointsExtensions
    {
        public static void UseDynamicEndpoints(this WebApplication app)
        {
            // Add the dynamic endpoint middleware
            app.Use(async (context, next) =>
            {
                // Get an instance of EndpointService from the dependency injection container
                var endpointService = app.Services.GetRequiredService<EndpointService>();

                // Check if the request path matches the path of any dynamic endpoint
                var endpointConfig = endpointService.GetEndpointConfiguration(context.Request.Path);

                if (endpointConfig != null)
                {
                    // Get an instance of ILogger from the dependency injection container
                    var logger = app.Services.GetRequiredService<ILogger<EndpointService>>();

                    // Log the details of the endpoint configuration
                    logger.LogInformation($"Found matching dynamic endpoint: {endpointConfig.Path}");
                    logger.LogInformation($"Executor: {endpointConfig.Executor}");

                    if (endpointConfig.Executor == "bash")
                    {
                        // Get an instance of EndpointExecutor from the dependency injection container
                        var executor = app.Services.GetRequiredService<EndpointExecutor>();

                        // Get the executor configuration
                        var executorConfig = endpointService.GetExecutorConfiguration(endpointConfig.Executor);

                        // Execute the command and get the output
                        var output = await executor.ExecuteCommand(executorConfig);

                        // Log the output
                        logger.LogInformation($"Output: {output}");

                        // Write the output to the response
                        await context.Response.WriteAsync(output);
                    }
                    else
                    {
                        logger.LogInformation($"Executor type {endpointConfig.Executor} not supported.");
                    }
                }
                else
                {
                    // Pass the request to the next middleware in the pipeline
                    await next.Invoke();
                }
            });
        }
    }
}