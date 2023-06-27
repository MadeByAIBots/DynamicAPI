using HelloWorldAPIProject;
using HelloWorldAPIProject.Definitions.ExecutorDefinitions;
using System;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

public class BashEndpointExecutor : IEndpointExecutor
{
    private readonly ILogger<BashEndpointExecutor> _logger;

    public BashEndpointExecutor(ILoggerFactory loggerFactory)
    {
        _logger = new Logger<BashEndpointExecutor>(loggerFactory);
        _logger.LogInformation("BashEndpointExecutor initialized.");
    }

    /*public BashEndpointExecutor(ILogger<BashEndpointExecutor> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogInformation("BashEndpointExecutor initialized.");
    }*/

    public async Task<string> ExecuteCommand(IExecutorConfiguration executorConfig)
    {
        _logger.LogInformation("Executing command...");

        var bashExecutorConfig = (BashExecutorConfiguration)executorConfig;
        try
        {
            if (bashExecutorConfig == null)
            {
                _logger.LogError("The configuration is null.");
                throw new ArgumentNullException(nameof(bashExecutorConfig));
            }

            string command = bashExecutorConfig.Command;

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