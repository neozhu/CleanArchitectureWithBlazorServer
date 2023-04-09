namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
public class ServerExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : IRequest<Result>
    where TException : ServerException
{
    private readonly ILogger<ServerExceptionHandler<TRequest, TResponse, TException>> _logger;

    public ServerExceptionHandler(ILogger<ServerExceptionHandler<TRequest, TResponse, TException>> logger)
    {
        _logger = logger;
    }

    public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state, CancellationToken cancellationToken)
    {
        var response = Activator.CreateInstance<TResponse>();
        if (response is Result result)
        {
            result.Succeeded = false;
            result.Errors = new string[] { exception.Message };
            state.SetHandled(response);
        }
        return Task.CompletedTask;
    }


}
