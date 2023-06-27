using HelloWorldAPIProject;
using HelloWorldAPIProject.Definitions.ExecutorDefinitions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class RequestProcessor
{
    private readonly ILogger<RequestProcessor> _logger;

    public RequestProcessor(ILogger<RequestProcessor> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ProcessRequest(HttpContext context, IEndpointExecutor executor, BashExecutorConfiguration configuration)
    {
        _logger.LogInformation("Processing request...");
        _logger.LogInformation($"Request method: {context.Request.Method}");
        _logger.LogInformation($"Request path: {context.Request.Path}");
        _logger.LogInformation($"Request query string: {context.Request.QueryString}");
        _logger.LogInformation($"Request headers: {context.Request.Headers}");

        _logger.LogInformation($"Executor type: {executor.GetType().Name}");

        try
        {
            _logger.LogInformation("Executing command...");
            var result = await executor.ExecuteCommand(configuration);
            _logger.LogInformation($"Command executed. Result: {result}");

            _logger.LogInformation("Writing response...");
            await context.Response.WriteAsync(result);
            _logger.LogInformation("Response written.");
        }
        catch (SystemException ex)
        {
            _logger.LogError(ex, "A system exception occurred during request processing.");
            await context.Response.WriteAsync($"System error: {ex.Message}");
        }
        catch (ApplicationException ex)
        {
            _logger.LogError(ex, "An application exception occurred during request processing.");
            await context.Response.WriteAsync($"Application error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred during request processing.");
            await context.Response.WriteAsync($"Error: {ex.Message}");
        }

        _logger.LogInformation("Request processed successfully.");
    }
}