public abstract class DynamicEndpointExecutorBase : IDynamicEndpointExecutor
{
    public abstract Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters);

    protected EndpointExecutionResult Result()
    {
        return new EndpointExecutionResult();
    }
    protected EndpointExecutionResult Success()
    {
        return Result().Success();
    }
protected EndpointExecutionResult Success(string message)
{
    return Result().Success().WithBody(message);
}
protected EndpointExecutionResult Success(object body)
{
    return Result().Success().WithBody(JsonSerializer.Serialize(body));
}
    }
