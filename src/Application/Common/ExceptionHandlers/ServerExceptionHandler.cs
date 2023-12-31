namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

public class
    ServerExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : IRequest<Result<int>>
    where TResponse : Result<int>
    where TException : ServerException
{
    private readonly ILogger<ServerExceptionHandler<TRequest, TResponse, TException>> _logger;

    public ServerExceptionHandler(ILogger<ServerExceptionHandler<TRequest, TResponse, TException>> logger)
    {
        _logger = logger;
    }

    public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken)
    {
        state.SetHandled((TResponse)Result<int>.Failure(exception.Message));
        return Task.CompletedTask;
    }
}