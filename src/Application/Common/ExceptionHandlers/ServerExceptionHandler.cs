namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

public class
    ServerExceptionHandler<TRequest, TResponse> : MessageExceptionHandler<TRequest, TResponse>
    where TRequest : IRequest<Result<int>>
    where TResponse : Result<int>
{
    private readonly ILogger<ServerExceptionHandler<TRequest, TResponse>> _logger;

    public ServerExceptionHandler(ILogger<ServerExceptionHandler<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    protected override ValueTask<ExceptionHandlingResult<TResponse>> Handle(TRequest request, Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ServerException serverException)
        {
            return NotHandled;
        }

        _logger.LogError(serverException, serverException.Message);
        return Handled((TResponse)Result<int>.Failure(serverException.Message));
    }
}
