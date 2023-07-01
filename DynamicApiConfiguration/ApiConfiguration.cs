namespace DynamicApiConfiguration;

public class ApiConfiguration
{
    public string Url { get; set; }
    public int Port { get; set; }
    public string EndpointPath { get; set; }
    public string TokenFilePath { get; set; }
    public CSharpScriptConfiguration CSharp { get; set; }
}