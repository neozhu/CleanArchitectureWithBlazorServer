
namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

public sealed class
    ValidationExceptionHandler<TRequest, TResponse, TException>
    where TRequest : IRequest<TResponse>
    where TResponse : IResult
    where TException : ValidationException
{

    public ValueTask Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken)
    {
        var errors = exception.Errors.Select(x => x.ErrorMessage).Distinct().ToArray();
        var failureResult = CreateFailureResult(errors);
        state.SetHandled(failureResult);
        return ValueTask.CompletedTask;
    }

    private TResponse CreateFailureResult(string[] errors)
    {
        return ResultFailureFactory.Create<TResponse>(errors);
    }
}
