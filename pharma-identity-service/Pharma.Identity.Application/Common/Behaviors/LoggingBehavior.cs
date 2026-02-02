using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Pharma.Identity.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestFullName = typeof(TRequest).FullName;
        var stopwatch = Stopwatch.StartNew();

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Starting request {RequestName} ({RequestFullName}) with data: {@Request}", 
                requestName, requestFullName, request);
        }

        try
        {
            var response = await next(cancellationToken);

            stopwatch.Stop();

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Completed request {RequestName} in {ElapsedMs}ms",
                    requestName, stopwatch.ElapsedMilliseconds);
            }

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError(ex, "Request {RequestName} failed after {ElapsedMs}ms",
                    requestName, stopwatch.ElapsedMilliseconds);
            }

            throw;
        }
    }
}