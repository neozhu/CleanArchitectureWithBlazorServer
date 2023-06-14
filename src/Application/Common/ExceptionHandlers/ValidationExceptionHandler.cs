namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
public class ValidationExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : IRequest<Result>
    where TException : ValidationException
{
    private readonly ILogger<ValidationExceptionHandler<TRequest, TResponse, TException>> _logger;

    public ValidationExceptionHandler(ILogger<ValidationExceptionHandler<TRequest, TResponse, TException>> logger)
    {
        _logger = logger;
    }

    public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state, CancellationToken cancellationToken)
    {
        var response = Activator.CreateInstance<TResponse>();
        if(response is Result result)
        {
 
            result = new Result { Succeeded = false, Errors = exception.Errors.Select(x => x.ErrorMessage).Distinct().ToArray() };
            state.SetHandled((TResponse)(object)result);
        }
        return Task.CompletedTask;
    }
}
