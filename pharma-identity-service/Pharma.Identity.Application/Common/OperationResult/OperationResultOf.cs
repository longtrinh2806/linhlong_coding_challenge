using System.Net;

namespace Pharma.Identity.Application.Common.OperationResult;

public class OperationResult<T> : BaseOperationResult
{
    private OperationResult(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
    }

    public OperationResult(HttpStatusCode statusCode, T value)
    {
        StatusCode = statusCode;
        Value = value;
    }

    public T? Value { get; private set; }

    public static implicit operator OperationResult<T>(OperationResult operationResult)
    {
        return new OperationResult<T>(operationResult.StatusCode)
        {
            Errors = operationResult.Errors
        };
    }
}