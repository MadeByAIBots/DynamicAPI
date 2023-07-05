using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Logging;
using System;

public class CSharpScriptResultHandler
{
    private readonly ILogger _logger;

    public CSharpScriptResultHandler(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<CSharpScriptResultHandler>();
    }

public string HandleResult(ScriptState<object> scriptState)
    {
        _logger.LogInformation("Handling script result...");

        if (scriptState.Exception != null)
        {
            _logger.LogError($"Script execution failed: {scriptState.Exception.Message}");
            throw new Exception("Script execution failed.", scriptState.Exception);
        }

        _logger.LogInformation("Script result handled successfully.");

return scriptState.ReturnValue.ToString();
    }
}
