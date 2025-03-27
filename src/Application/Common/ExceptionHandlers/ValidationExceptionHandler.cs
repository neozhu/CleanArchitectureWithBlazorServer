namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

public class
    ValidationExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse,
    TException>
    where TRequest : IRequest<Result>
    where TResponse : Result
    where TException : ValidationException
{

    public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken)
    {
        state.SetHandled(
            (TResponse)Result.Failure(exception.Errors.Select(x => x.ErrorMessage).Distinct().ToArray()));
        return Task.CompletedTask;
    }
}