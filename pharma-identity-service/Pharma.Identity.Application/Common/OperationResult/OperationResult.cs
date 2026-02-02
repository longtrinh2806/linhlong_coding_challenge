using System.Net;

namespace Pharma.Identity.Application.Common.OperationResult;

public class OperationResult : BaseOperationResult
{
    private OperationResult(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
    }

    public static OperationResult Ok() => new OperationResult(HttpStatusCode.OK);

    public static OperationResult BadRequest(string? message = null)
        => new(HttpStatusCode.BadRequest) { Errors = { { "message", message } } };

    public static OperationResult BadRequest(IDictionary<string, string?> errors)
        => new(HttpStatusCode.BadRequest) { Errors = errors };

    public static OperationResult NoContent(string? message = null)
        => new(HttpStatusCode.NoContent) { Errors = { { "message", message } } };
    
    public static OperationResult NotFound(string? message = null)
        => new(HttpStatusCode.NotFound) { Errors = { { "message", message } } };

    public static OperationResult Forbidden(string? message = null)
        => new(HttpStatusCode.Forbidden) { Errors = { { "message", message } } };

    public static OperationResult Unauthorized(string? message = null)
        => new(HttpStatusCode.Unauthorized) { Errors = { { "message", message } } };

    public static OperationResult Status(HttpStatusCode statusCode) => new(statusCode);

    public static OperationResult<T> Ok<T>(T value) => new OperationResult<T>(HttpStatusCode.OK, value);
}