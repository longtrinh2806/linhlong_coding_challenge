using System.Net;
using Pharma.Identity.Application.Common.OperationResult;

namespace Pharma.Identity.API.Extensions;

public static class OperationResultExtension
{
    public static IResult ToResult(this OperationResult operationResult)
    {
        return Results.Json(
            operationResult,
            statusCode: (int)operationResult.StatusCode
        );
    }

    public static IResult ToResult<T>(this OperationResult<T> operationResult)
    {
        return operationResult.StatusCode switch
        {
            HttpStatusCode.OK => Results.Ok(operationResult),
            HttpStatusCode.NoContent => Results.NoContent(),
            HttpStatusCode.BadRequest => Results.BadRequest(operationResult),
            HttpStatusCode.Forbidden => Results.Forbid(),
            HttpStatusCode.Unauthorized => Results.Unauthorized(),
            _ => Results.StatusCode((int)operationResult.StatusCode)
        };
    }
}