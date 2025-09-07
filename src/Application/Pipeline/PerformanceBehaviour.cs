using System.Diagnostics;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

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
    private readonly IUserContextAccessor _userContextAccessor;
    private readonly ILogger<PerformanceBehaviour<TRequest, TResponse>> _logger;

    public PerformanceBehaviour(
        ILogger<PerformanceBehaviour<TRequest, TResponse>> logger,
        IUserContextAccessor userContextAccessor)
    {
        _logger = logger;
        _userContextAccessor = userContextAccessor;
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
        var timer = Stopwatch.StartNew();

        // Increment ExecutionCount in a thread-safe manner.
        Interlocked.Increment(ref RequestCounter.ExecutionCount);

        var response = await next().ConfigureAwait(false);

        timer.Stop();
        var elapsedMilliseconds = timer.ElapsedMilliseconds;

        // Use higher threshold during startup (first 50 requests or 60 seconds)
        var isStartupPhase = RequestCounter.ExecutionCount <= 50 || 
                            (DateTime.UtcNow - RequestCounter.StartTime).TotalSeconds < 60;
        
        var threshold = isStartupPhase ? 2000 : 500;

        if (elapsedMilliseconds > threshold)
        {
            var requestName = typeof(TRequest).Name;
            var userName = _userContextAccessor.Current?.UserName;
            var phase = isStartupPhase ? "Startup" : "Runtime";

            _logger.LogWarning(
                "Long-running request [{Phase}]: {RequestName} ({ElapsedMilliseconds}ms) {@Request} by {UserName}",
                phase, requestName, elapsedMilliseconds, request, userName);
        }

        return response;
    }
}

/// <summary>
///     Static class that holds the ExecutionCount and StartTime in a shared context between different
///     instances of our PerformanceBehaviour class, regardless of the type of TRequest.
/// </summary>
public static class RequestCounter
{
    public static int ExecutionCount;
    public static readonly DateTime StartTime = DateTime.UtcNow;
}