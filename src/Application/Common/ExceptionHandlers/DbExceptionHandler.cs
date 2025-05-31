using EntityFramework.Exceptions.Common;

namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

/// <summary>
/// Handles database update exceptions and converts them into Result or Result&lt;T&gt; responses.
/// </summary>
public class DbExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : IRequest<Result>
    where TResponse : Result
    where TException : DbUpdateException
{
    private readonly ILogger _logger;
    private readonly ILoggerFactory _loggerFactory;

    public DbExceptionHandler(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger(nameof(DbExceptionHandler<TRequest, TResponse, TException>));
    }

    public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken)
    {
        var errors = GetErrors(exception);

        // If TResponse is a generic Result<T>, create the failure result dynamically.
        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = typeof(TResponse).GetGenericArguments()[0];
            var failureMethod = typeof(Result<>).MakeGenericType(valueType).GetMethod("Failure", new[] { typeof(string[]) });
            if (failureMethod is null)
            {
                throw new InvalidOperationException("Could not find the 'Failure' method on Result<>.");
            }
            var resultObj = failureMethod.Invoke(null, new object[] { errors });
            if (resultObj is null)
            {
                throw new InvalidOperationException("The 'Failure' method returned null.");
            }
            var result = (TResponse)resultObj;
            state.SetHandled(result);
        }
        else
        {
            // For non-generic Result
            state.SetHandled((TResponse)(object)Result.Failure(errors));
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Maps specific database exceptions to user-friendly error messages.
    /// </summary>
    private string[] GetErrors(DbUpdateException exception)
    {
        return exception switch
        {
            UniqueConstraintException e => GetUniqueConstraintExceptionErrors(e),
            CannotInsertNullException e => GetCannotInsertNullExceptionErrors(e),
            MaxLengthExceededException e => GetMaxLengthExceededExceptionErrors(e),
            NumericOverflowException e => GetNumericOverflowExceptionErrors(e),
            ReferenceConstraintException e => GetReferenceConstraintExceptionErrors(e),
            _ => new[] { exception.GetBaseException().Message }
        };
    }

    private string[] GetUniqueConstraintExceptionErrors(UniqueConstraintException exception)
    {
        var tableName = string.IsNullOrWhiteSpace(exception.SchemaQualifiedTableName) ? "unknown table" : exception.SchemaQualifiedTableName;
        var properties = exception.ConstraintProperties != null && exception.ConstraintProperties.Any()
            ? string.Join(", ", exception.ConstraintProperties)
            : "unknown properties";

        return new[]
        {
            $"A unique constraint violation occurred on constraint in table '{tableName}'. " +
            $"'{properties}'. Please ensure the values are unique."
        };
    }

    private string[] GetCannotInsertNullExceptionErrors(CannotInsertNullException exception)
    {
        return new[]
        {
            "Some required information is missing. Please make sure all required fields are filled out."
        };
    }

    private string[] GetMaxLengthExceededExceptionErrors(MaxLengthExceededException exception)
    {
        return new[]
        {
            "Some input is too long. Please shorten the data entered in the fields."
        };
    }

    private string[] GetNumericOverflowExceptionErrors(NumericOverflowException exception)
    {
        return new[]
        {
           "A number you entered is too large or too small. Please enter a number within the allowed range."
        };
    }

    private string[] GetReferenceConstraintExceptionErrors(ReferenceConstraintException exception)
    {
        var tableName = string.IsNullOrWhiteSpace(exception.SchemaQualifiedTableName) ? "unknown table" : exception.SchemaQualifiedTableName;
        return new[]
        {
            $"The operation failed because this record is linked to other records in {tableName}. " +
            $"Please remove any related records first"
        };
    }
}