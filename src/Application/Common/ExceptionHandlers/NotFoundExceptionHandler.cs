namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

/// <summary>
/// Handles NotFoundException and converts them into Result or Result&lt;T&gt; responses.
/// Provides user-friendly error messages for entity not found scenarios.
/// </summary>
public sealed class NotFoundExceptionHandler<TRequest, TResponse, TException>
    where TRequest : IRequest<TResponse>
    where TResponse : IResult
    where TException : NotFoundException
{
    private readonly ILogger<NotFoundExceptionHandler<TRequest, TResponse, TException>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundExceptionHandler{TRequest, TResponse, TException}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public NotFoundExceptionHandler(ILogger<NotFoundExceptionHandler<TRequest, TResponse, TException>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the NotFoundException and sets the failure result.
    /// </summary>
    /// <param name="request">The request that caused the exception.</param>
    /// <param name="exception">The NotFoundException to handle.</param>
    /// <param name="state">The request exception handler state.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public ValueTask Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken)
    {
         
            var failureResult = CreateFailureResult(exception.Message);
            state.SetHandled(failureResult);
            
            _logger.LogError(exception, 
                "NotFoundException occurred for request {RequestType}: {ErrorMessage}", 
                typeof(TRequest).Name, 
                exception.Message);
        
        
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Creates a failure result of the appropriate type.
    /// </summary>
    /// <param name="errorMessage">The error message to include in the result.</param>
    /// <returns>A failure result of type TResponse.</returns>
    private TResponse CreateFailureResult(string errorMessage)
    {
        return ResultFailureFactory.Create<TResponse>(errorMessage);
    }
}
