namespace DynamicApi.Configuration;

public class ApiConfiguration
{
    
    public string Url { get; set; }

    public string ExternalUrl { get; set; }
    
    // TODO: Remove if not needed. Auto generating it seems like a good idea but it keeps messing it up. Either fix it or remove it.
    // private string externalUrl;
    // public string ExternalUrl
    // {
    //     get
    //     {
    //         if (String.IsNullOrEmpty(externalUrl))
    //             return Url + ":" + Port.ToString();
    //
    //         return externalUrl;
    //     }
    //     set { externalUrl = value; }
    // }
    
    public int Port { get; set; }
    public string NamePostfix { get; set; }
    public string EndpointPath { get; set; }
    public string TokenFilePath { get; set; }
    
    public string OpenAIVerificationToken { get; set; }
    public CSharpScriptConfiguration CSharpScript { get; set; }
    public Dictionary<string, string> Variables { get; set; }
    public string[] StaticFilePaths { get; set; } = new string[] { "/openapi.yaml", "/.well-known/ai-plugin.json" };
}
