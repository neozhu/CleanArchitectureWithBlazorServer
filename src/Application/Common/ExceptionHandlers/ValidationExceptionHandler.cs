namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

public class
    ValidationExceptionHandler<TRequest, TResponse> : MessageExceptionHandler<TRequest, TResponse>
    where TRequest : IRequest<Result<int>>
    where TResponse : Result<int>
{
    private readonly ILogger<ValidationExceptionHandler<TRequest, TResponse>> _logger;

    public ValidationExceptionHandler(ILogger<ValidationExceptionHandler<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    protected override ValueTask<ExceptionHandlingResult<TResponse>> Handle(TRequest request, Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            return NotHandled;
        }

        return Handled(
            (TResponse)Result<int>.Failure(validationException.Errors.Select(x => x.ErrorMessage).Distinct().ToArray()));
    }
}
