public class EndpointArgumentDefinition
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string Source { get; set; }
    public bool Required { get;set; } = true;
    public string Description { get; set; }
}