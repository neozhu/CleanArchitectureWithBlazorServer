namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

public class GlobalExceptionHandler<TRequest, TResponse> : MessageExceptionHandler<TRequest, TResponse>
    where TRequest : IRequest<IResult>
    where TResponse : IResult
{
    private readonly ILogger<GlobalExceptionHandler<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalExceptionHandler{TRequest, TResponse, TException}"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the exception and sets the failure result.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="exception">The exception.</param>
    /// <param name="state">The request exception handler state.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override ValueTask<ExceptionHandlingResult<TResponse>> Handle(TRequest request, Exception exception,
        CancellationToken cancellationToken)
    {
        TResponse failureResult;
        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            // Get the type parameter T in Result<T>
            var resultType = typeof(TResponse).GetGenericArguments()[0];

            // Use reflection to invoke Result<T>.Failure method
            var failureMethod = typeof(Result<>)
                .MakeGenericType(resultType)
                .GetMethod("Failure", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string[]) }, null);

            var failureResultObj = failureMethod?.Invoke(null, new object[] { new[] { exception.Message } });

            failureResult = (TResponse)(failureResultObj ?? throw new ArgumentNullException(nameof(failureResultObj)));
        }
        else
        {
            failureResult = (TResponse)(object)Result.Failure(exception.Message);
        }

        // Set the handled response
        _logger.LogError(exception, exception.Message);
        return Handled(failureResult!);
    }
}
