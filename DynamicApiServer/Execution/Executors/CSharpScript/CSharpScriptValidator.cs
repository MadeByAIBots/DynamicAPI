using System;
using System.Threading.Tasks;

namespace DynamicApiServer.Execution.Executors.CSharpScript
{
    public class CSharpScriptValidator
    {
        public async Task<bool> ValidateAsync(string script)
        {
            // TODO: Implement script validation logic here...

            // For now, just return true.
            return await Task.FromResult(true);
        }
    }
}