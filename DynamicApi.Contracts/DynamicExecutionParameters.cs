namespace DynamicApi.Contracts;

public class DynamicExecutionParameters
{
    public Dictionary<string, string> Parameters { get; set; }

    public DynamicExecutionParameters(Dictionary<string, string> parameters)
    {
        Parameters = parameters;
    }
}