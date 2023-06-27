using HelloWorldAPIProject;
using HelloWorldAPIProject.Definitions.ExecutorDefinitions;
using System;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

public class EndpointExecutor : IEndpointExecutor
{
    private readonly ILogger<EndpointExecutor> _logger;

    public EndpointExecutor(ILogger<EndpointExecutor> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogInformation("EndpointExecutor initialized.");
    }

    public async Task<string> ExecuteCommand(BashExecutorConfiguration configuration)
    {
        _logger.LogInformation("Executing command...");

        try
        {
            if (configuration == null)
            {
                _logger.LogError("The configuration is null.");
                throw new ArgumentNullException(nameof(configuration));
            }

            string command = configuration.Command;

            if (string.IsNullOrEmpty(command))
            {
                _logger.LogError("The command is null or empty.");
                throw new ArgumentException("Command cannot be null or empty");
            }

            _logger.LogInformation($"Starting execution of command: {command}");

            var processRunner = new ProcessRunner();
            var output = await processRunner.RunProcess(command);

            if (string.IsNullOrEmpty(output))
            {
                _logger.LogError("The output is null or empty.");
                throw new Exception("Output cannot be null or empty");
            }

            _logger.LogInformation($"Successfully executed command: {command}");
            _logger.LogInformation($"Command output: {output}");

            return output;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred during command execution.");
            return $"Error occurred during command execution: {ex.Message}";
        }
    }
}