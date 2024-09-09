using System.Diagnostics;

namespace CleanArchitecture.Blazor.Application.Pipeline;

/// <summary>
///     This class is a behavior pipeline in MediatR. It is used to monitor performance
///     and log warnings if a request takes longer to execute than a specified threshold.
/// </summary>
/// <typeparam name="TRequest">Type of the Request</typeparam>
/// <typeparam name="TResponse">Type of the Response</typeparam>
public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<PerformanceBehaviour<TRequest, TResponse>> _logger;

    public PerformanceBehaviour(
        ILogger<PerformanceBehaviour<TRequest, TResponse>> logger,
        ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    /// <summary>
    ///     Logs warnings if a request takes longer to execute than a specified threshold.
    /// </summary>
    /// <param name="request">The incoming request</param>
    /// <param name="next">The delegate for the next action in the pipeline process.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the next delegate</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        Stopwatch? timer = null;

        // Increment ExecutionCount in a thread-safe manner.
        Interlocked.Increment(ref RequestCounter.ExecutionCount);
        if (RequestCounter.ExecutionCount > 3) timer = Stopwatch.StartNew();

        var response = await next().ConfigureAwait(false);

        timer?.Stop();
        var elapsedMilliseconds = timer?.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;
            var userName = _currentUserService.UserName;

            _logger.LogWarning(
    "Long-running request detected: {RequestName} ({ElapsedMilliseconds}ms) {@Request} by {UserName}",
    requestName, elapsedMilliseconds, request, userName);
        }

        return response;
    }
}

/// <summary>
///     Static class that holds the ExecutionCount in a shared context between different
///     instances of our PerformanceBehaviour class, regardless of the type of TRequest.
///     This allows to keep track of the number of requests application-wide.
/// </summary>
public static class RequestCounter
{
    public static int ExecutionCount;
}