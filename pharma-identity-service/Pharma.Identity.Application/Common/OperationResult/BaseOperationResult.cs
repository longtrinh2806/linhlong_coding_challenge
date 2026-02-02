using System.Net;

namespace Pharma.Identity.Application.Common.OperationResult;

public abstract class BaseOperationResult
{
    public bool IsSuccess => (int)StatusCode >= 200 && (int)StatusCode < 299;

    public HttpStatusCode StatusCode { get; set; }

    public IDictionary<string, string?> Errors { get; set; } = new Dictionary<string, string?>();
}