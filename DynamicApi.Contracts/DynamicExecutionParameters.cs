using DynamicApiConfiguration;
using DynamicApiServer;

namespace DynamicApi.Contracts;

public class DynamicExecutionParameters
{
    public ApiConfiguration ApiConfig { get; set; }
    public WorkingDirectoryResolver Resolver { get;set;}
    public Dictionary<string, string> Parameters { get; set; }

    public DynamicExecutionParameters(ApiConfiguration apiConfig, WorkingDirectoryResolver resolver, Dictionary<string, string> parameters)
    {
        ApiConfig = apiConfig;
        Parameters = parameters;
        Resolver = resolver;
    }
}