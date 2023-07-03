namespace DynamicApi.Contracts;

public class DynamicExecutionParameters
{
    public ApiConfiguration ApiConfig { get; set; }
    public WorkingDirectoryResolver Resolver { get;set;}
    public Dictionary<string, string> Parameters { get; set; }

    public ILoggerFactory LoggerFactory { get;set;}

    public DynamicExecutionParameters(ApiConfiguration apiConfig, WorkingDirectoryResolver resolver, ILoggerFactory loggerFactory, Dictionary<string, string> parameters)
    {
        LoggerFactory = loggerFactory;
        ApiConfig = apiConfig;
        Parameters = parameters;
        Resolver = resolver;
    }
}