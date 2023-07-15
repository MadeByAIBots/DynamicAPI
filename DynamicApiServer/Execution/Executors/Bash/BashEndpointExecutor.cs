using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DynamicApi.Endpoints.Executors.Model;
using DynamicApi.Endpoints.Model;

namespace DynamicApiServer.Execution.Executors.Bash
{
    public class BashEndpointExecutor : IEndpointExecutor
    {
        private readonly ILogger<BashEndpointExecutor> _logger;
        private readonly BashCommandArgumentInjector _commandArgumentInjector;
        private readonly ProcessRunner _processRunner;

        public BashEndpointExecutor(ILoggerFactory loggerFactory, ProcessRunner processRunner)
        {
            _logger = loggerFactory.CreateLogger<BashEndpointExecutor>();
            _commandArgumentInjector = new BashCommandArgumentInjector(loggerFactory);
            _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
            _logger.LogInformation("BashEndpointExecutor initialized.");
        }
        public async Task<string> ExecuteCommand(EndpointDefinition endpointDefinition, IExecutorDefinition executorConfig, Dictionary<string, string> args)
        {
            if (executorConfig == null)
            {
                _logger.LogError("Executor configuration is null.");
                throw new ArgumentNullException(nameof(executorConfig));
            }

            if (args == null)
            {
                _logger.LogError("Arguments dictionary is null.");
                throw new ArgumentNullException(nameof(args));
            }

            var bashExecutorConfig = executorConfig as BashExecutorDefinition;
            if (bashExecutorConfig == null)
            {
                _logger.LogError("Executor configuration is not of type BashExecutorConfig.");
                throw new ArgumentException("Invalid executor configuration type.", nameof(executorConfig));
            }

            string command = bashExecutorConfig.Command;
            string commandWithArgs;
            try
            {
                commandWithArgs = _commandArgumentInjector.InjectArgumentsIntoCommand(command, args);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error injecting arguments into command.");
                throw;
            }

            string output;
            try
            {
                output = await _processRunner.RunProcess(commandWithArgs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running process.");
                throw;
            }

            return output;
        }
    }
}