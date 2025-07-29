namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

public sealed class
    ValidationExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse,
    TException>
    where TRequest : IRequest<TResponse>
    where TResponse : IResult
    where TException : ValidationException
{

    public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken)
    {
        var errors = exception.Errors.Select(x => x.ErrorMessage).Distinct().ToArray();
        var failureResult = CreateFailureResult(errors);
        state.SetHandled(failureResult);
        return Task.CompletedTask;
    }

    private TResponse CreateFailureResult(string[] errors)
    {
        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            // Get the type parameter T in Result<T>
            var resultType = typeof(TResponse).GetGenericArguments()[0];

            // Use reflection to invoke Result<T>.Failure method
            var failureMethod = typeof(Result<>)
                .MakeGenericType(resultType)
                .GetMethod("Failure", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string[]) }, null);

            if (failureMethod != null)
            {
                return (TResponse)failureMethod.Invoke(null, new object[] { errors })!;
            }
        }
        else
        {
            // For non-generic Result type
            var failureMethod = typeof(Result).GetMethod("Failure", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string[]) }, null);
            if (failureMethod != null)
            {
                return (TResponse)failureMethod.Invoke(null, new object[] { errors })!;
            }
        }

        throw new InvalidOperationException($"Could not create failure result for type {typeof(TResponse).Name}");
    }
}