using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using DynamicApiServer.Definitions.ExecutorDefinitions;
using DynamicApiConfiguration;

namespace DynamicApiServer.Execution.Executors.CSharpScript
{
    public static class CSharpScriptUtilities
    {
        public static void ValidateInput(IExecutorDefinition executorConfig, Dictionary<string, string> args)
        {
            if (executorConfig == null)
            {
                throw new ArgumentNullException(nameof(executorConfig));
            }

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }
        }

        public static string FindScript(string scriptName, string folderName, ApiConfiguration _apiConfig)
        {
            string scriptPath = Path.Combine(_apiConfig.EndpointPath, folderName, scriptName);

            if (!File.Exists(scriptPath))
            {
                throw new FileNotFoundException($"Script file not found: {scriptPath}");
            }

            return scriptPath;
        }

        public static void ValidateScript(string scriptPath)
        {
            // Verification of script compilation is currently disabled
            // TODO: Add script verification logic here
        }

        public static async Task<string> ExecuteScript(string scriptPath, Dictionary<string, string> args)
        {
            var script = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.Create(File.ReadAllText(scriptPath), globalsType: typeof(Dictionary<string, string>));
            var result = await script.RunAsync(args);

            if (result.Exception != null)
            {
                throw new Exception("Script execution failed.", result.Exception);
            }

            return result.ReturnValue?.ToString() ?? string.Empty;
        }
    }
}