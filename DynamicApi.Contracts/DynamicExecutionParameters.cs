namespace DynamicApi.Contracts;

public class DynamicExecutionParameters
{
    public ApiConfiguration ApiConfig { get; set; }
    public WorkingDirectoryResolver Resolver { get; set; }
    public Dictionary<string, string> Parameters { get; set; }

    public ILoggerFactory LoggerFactory { get; set; }

    public DynamicExecutionParameters(ApiConfiguration apiConfig, WorkingDirectoryResolver resolver, ILoggerFactory loggerFactory, Dictionary<string, string> parameters)
    {
        LoggerFactory = loggerFactory;
        ApiConfig = apiConfig;
        Parameters = parameters;
        Resolver = resolver;
    }
    public string GetRequiredString(string paramName)
    {
        if (Parameters.ContainsKey(paramName) && Parameters[paramName] is string stringValue)
        {
            return stringValue;
        }
        else
        {
            throw new ArgumentException($"Required parameter '{paramName}' is missing or not a string.");
        }
    }
    public string GetString(string paramName)
    {
        if (Parameters.ContainsKey(paramName) && Parameters[paramName] is string stringValue)
        {
            return stringValue;
        }
        else
        {
            return string.Empty;
        }
    }
    public int GetRequiredInt32(string paramName)
    {
        if (Parameters.ContainsKey(paramName) && int.TryParse(Parameters[paramName]?.ToString(), out var intValue))
        {
            return intValue;
        }
        else
        {
            throw new ArgumentException($"Required parameter '{paramName}' is missing or not an integer.");
        }
    }
    public int GetInt32(string paramName)
    {
        if (Parameters.ContainsKey(paramName) && int.TryParse(Parameters[paramName]?.ToString(), out var intValue))
        {
            return intValue;
        }
        else
        {
            return 0;
        }
    }
    public bool GetRequiredBool(string paramName)
    {
        if (Parameters.ContainsKey(paramName) && bool.TryParse(Parameters[paramName]?.ToString(), out var boolValue))
        {
            return boolValue;
        }
        else
        {
            throw new ArgumentException($"Required parameter '{paramName}' is missing or not a bool.");
        }
    }
    public bool GetBool(string paramName, bool defaultValue = false)
    {
        if (Parameters.ContainsKey(paramName) && bool.TryParse(Parameters[paramName]?.ToString(), out var boolValue))
        {
            return boolValue;
        }
        else
        {
            return defaultValue;
        }
    }
}
