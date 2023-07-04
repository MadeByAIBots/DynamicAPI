namespace DynamicApi.Contracts;

public class EndpointExecutionResult
{
    public bool IsSuccess { get; private set; }
    public string Body { get; private set; }

    public EndpointExecutionResult WithIsSuccess(bool isSuccess)
    {
        IsSuccess = isSuccess;
        return this;
    }

    public EndpointExecutionResult WithBody(string body)
    {
        Body = body;
        return this;
    }

    public EndpointExecutionResult Success()
    {
        return WithIsSuccess(true);
    }

    public EndpointExecutionResult Success(string body)
    {
        return WithIsSuccess(true).WithBody(body);
    }

    public EndpointExecutionResult Success(object body)
    {
        return WithIsSuccess(true).WithBody(JsonSerializer.Serialize(body));
    }

    public EndpointExecutionResult Fail()
    {
        return WithIsSuccess(false);
    }

    public EndpointExecutionResult Fail(string body)
    {
        return WithIsSuccess(false).WithBody(body);
    }

    public EndpointExecutionResult Fail(object body)
    {
        return WithIsSuccess(false).WithBody(JsonSerializer.Serialize(body));
    }
}