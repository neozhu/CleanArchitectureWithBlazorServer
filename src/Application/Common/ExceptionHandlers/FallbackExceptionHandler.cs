namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

public sealed class FallbackExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : IRequest<TResponse>
    where TResponse : IResult
    where TException : Exception
{
    private readonly ILogger<FallbackExceptionHandler<TRequest, TResponse, TException>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalExceptionHandler{TRequest, TResponse, TException}"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public FallbackExceptionHandler(ILogger<FallbackExceptionHandler<TRequest, TResponse, TException>> logger)
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
    public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken)
    {
        TResponse failureResult;
        string[] errorMessages;

        // Handle specific exception types with custom error messages
        switch (exception)
        {
            case NotFoundException notFoundEx:
                errorMessages = new[] { notFoundEx.Message };
                _logger.LogWarning(notFoundEx, "Entity not found: {Message}", notFoundEx.Message);
                break;

            case ValidationException validationEx:
                var validationErrors = validationEx.Errors?
                    .Select(error => $"{error.PropertyName}: {error.ErrorMessage}")
                    .ToArray() ?? new[] { "Validation failed" };
                errorMessages = validationErrors.Any() ? validationErrors : new[] { "Validation failed with unknown errors" };
                _logger.LogWarning(validationEx, "Validation failed with {ErrorCount} errors", validationErrors.Length);
                break;

            default:
                errorMessages = new[] { $"An unexpected error occurred: {exception.Message}" };
                _logger.LogError(exception, "Unhandled exception occurred: {ExceptionType}", exception.GetType().Name);
                break;
        }

        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            // Get the type parameter T in Result<T>
            var resultType = typeof(TResponse).GetGenericArguments()[0];

            // Use reflection to invoke Result<T>.Failure method
            var failureMethod = typeof(Result<>)
                .MakeGenericType(resultType)
                .GetMethod("Failure", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string[]) }, null);

            var failureResultObj = failureMethod?.Invoke(null, new object[] { errorMessages });

            failureResult = (TResponse)(failureResultObj ?? throw new ArgumentNullException(nameof(failureResultObj)));
        }
        else
        {
            failureResult = (TResponse)(object)Result.Failure(errorMessages);
        }

        // Set the handled response
        state.SetHandled(failureResult!);
        return Task.CompletedTask;
    }
}
