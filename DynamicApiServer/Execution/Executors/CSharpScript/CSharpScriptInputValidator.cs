using Microsoft.Extensions.Logging;
using System;

public class CSharpScriptValidator
{
    private readonly ILogger _logger;

public CSharpScriptValidator(ILoggerFactory loggerFactory)
    {
_logger = loggerFactory.CreateLogger<CSharpScriptValidator>();
    }

    public void ValidateInput(string scriptPath, object globals)
    {
        _logger.LogInformation("Validating input parameters...");

        if (string.IsNullOrWhiteSpace(scriptPath))
        {
            _logger.LogError("Script path is null or empty.");
            throw new ArgumentException("Script path cannot be null or empty.", nameof(scriptPath));
        }

        if (globals == null)
        {
            _logger.LogError("Globals object is null.");
            throw new ArgumentNullException(nameof(globals), "Globals object cannot be null.");
        }

        _logger.LogInformation("Input parameters are valid.");
    }
}
