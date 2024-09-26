namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

public class
    GlobalExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : IRequest<IResult>
    where TResponse : IResult
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
        TResponse failureResult;
        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            // Get the type parameter T in Result<T>
            var resultType = typeof(TResponse).GetGenericArguments()[0];

            // Use reflection to invoke Result<T>.Failure method
            var failureMethod = typeof(Result<>)
                .MakeGenericType(resultType)
                .GetMethod("Failure", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string[]) }, null);

            var failureResultObj = failureMethod.Invoke(null, new[] { exception.Message } );

            failureResult = (TResponse)failureResultObj;
        }
        else
        {
            failureResult = (TResponse)(object)Result.Failure(exception.Message);
        }

        // Set the handled response
        state.SetHandled(failureResult);
        _logger.LogError(exception, exception.Message);
        return Task.CompletedTask;
    }
}