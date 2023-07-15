using System.Collections.Generic;

namespace DynamicApi.Endpoints.Model
{
    public class EndpointDefinition
    {
        public string Path { get; set; }
        public string Method { get; set; }
        public string FolderName { get; set; }
        public List<EndpointResponseDefinition> Responses { get; set; }
        public string Executor { get; set; }
        public List<EndpointArgumentDefinition> Args { get; set; }
        public string Description { get; set; }
    }
}