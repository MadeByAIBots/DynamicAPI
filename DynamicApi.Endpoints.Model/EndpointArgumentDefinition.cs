using System.Text.Json.Serialization;

public class EndpointArgumentDefinition
{
    /// <summary>
    /// The name of the argument.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The value type of the argument such as: string, int, bool
    /// </summary>
    public string Type { get; set; } = "string";
    /// <summary>
    /// The source of the argument such as: body, query
    /// </summary>
    public string Source { get; set; }
    /// <summary>
    /// The format of the request body such as: string, json 
    /// </summary>
    [JsonPropertyName("sourceFormat")]
    public string SourceFormat { get; set; } = "json";
    /// <summary>
    /// Whether or not the argument is required.
    /// </summary>
    public bool Required { get;set; } = true;
    /// <summary>
    /// A description of the argument.
    /// </summary>
    public string Description { get; set; }
}