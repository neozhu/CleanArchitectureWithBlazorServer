namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

public class
    GlobalExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : IRequest<Result<int>>
    where TResponse : Result<int>
    where TException : Exception
{
    private readonly ILogger<GlobalExceptionHandler<TRequest, TResponse, TException>> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler<TRequest, TResponse, TException>> logger)
    {
        _logger = logger;
    }

    public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken)
    {
        state.SetHandled((TResponse)Result<int>.Failure(exception.Message));
        _logger.LogError(exception, exception.Message);
        return Task.CompletedTask;
    }
}