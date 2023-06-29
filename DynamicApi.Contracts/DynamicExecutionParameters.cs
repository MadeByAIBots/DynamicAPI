using DynamicApiConfiguration;

namespace DynamicApi.Contracts;

public class DynamicExecutionParameters
{
    public ApiConfiguration ApiConfig { get; set; }
    public Dictionary<string, string> Parameters { get; set; }

    public DynamicExecutionParameters(ApiConfiguration apiConfig, Dictionary<string, string> parameters)
    {
        ApiConfig = apiConfig;
        Parameters = parameters;
    }
}