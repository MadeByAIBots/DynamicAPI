namespace DynamicApiServer.Definitions.ExecutorDefinitions
{
    public class CSharpExecutorDefinition : IExecutorDefinition
    {
        public string Assembly { get; set; }
        public string Class { get; set; }
    }
}