namespace DynamicApi.Endpoints.Executors.Model
{
    public class CSharpExecutorDefinition : IExecutorDefinition
    {
        public string Assembly { get; set; }
        public string Class { get; set; }
    }
}