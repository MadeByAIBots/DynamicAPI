using System.Collections.Generic;

namespace DynamicApiServer.Definitions.EndpointDefinitions
{
    public class EndpointDefinition
    {
        public string Path { get; set; }
        public string FolderName { get;set; }
        public string Executor { get; set; }
        public List<EndpointArgumentDefinition> Args { get; set; }
    }
}
