using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class CSharpScriptCompiler
{
    private readonly ILogger _logger;

    public CSharpScriptCompiler(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<CSharpScriptCompiler>();
    }

    public async Task<ScriptState<object>> CompileScriptAsync(Script<object> script, object globals)
    {
        _logger.LogInformation("Compiling script...");

        ScriptState<object> scriptState;

        try
        {
            scriptState = await script.RunAsync(globals);
        }
        catch (CompilationErrorException ex)
        {
            _logger.LogError($"Script compilation failed: {ex.Message}");
            throw new Exception("Script compilation failed.", ex);
        }

        _logger.LogInformation("Script compiled successfully.");

        return scriptState;
    }
}