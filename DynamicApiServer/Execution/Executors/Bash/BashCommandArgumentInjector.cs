using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace DynamicApiServer.Execution.Executors.Bash
{
    public class BashCommandArgumentInjector
    {
        private readonly ILogger<BashCommandArgumentInjector> _logger;

        public BashCommandArgumentInjector(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<BashCommandArgumentInjector>();
        }

        public string InjectArgumentsIntoCommand(string command, Dictionary<string, string> args)
        {
            if (command == null)
            {
                _logger.LogError("Command string is null.");
                throw new ArgumentNullException(nameof(command));
            }

            if (args == null)
            {
                _logger.LogWarning("Argument dictionary is null. No arguments will be injected.");
                return command;
            }

            // For each argument in the dictionary
            foreach (var arg in args)
            {
                // Replace the placeholder in the command string with the argument value
                // The placeholder is assumed to be the argument key surrounded by ${ and }
                command = command.Replace($"${{{arg.Key}}}", arg.Value);
            }

            _logger.LogInformation("Arguments successfully injected into command.");

            // Return the command string with the arguments injected
            return command;
        }
    }
}
