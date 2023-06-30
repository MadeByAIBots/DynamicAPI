namespace DynamicApiServer.Definitions.EndpointDefinitions
{
    public class EndpointResponseDefinition
    {
        public int StatusCode { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } // e.g., "string", "integer", "object", etc.
        // Add more properties as needed to represent the structure of a response
    }
}