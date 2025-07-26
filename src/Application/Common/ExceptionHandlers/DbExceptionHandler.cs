using EntityFramework.Exceptions.Common;
using System.Reflection;

namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

/// <summary>
/// Handles database update exceptions and converts them into Result or Result&lt;T&gt; responses.
/// Provides user-friendly error messages for various database constraint violations.
/// </summary>
public class DbExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
    where TException : DbUpdateException
{
    private readonly ILogger<DbExceptionHandler<TRequest, TResponse, TException>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbExceptionHandler{TRequest, TResponse, TException}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public DbExceptionHandler(ILogger<DbExceptionHandler<TRequest, TResponse, TException>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles database update exceptions and creates appropriate failure results.
    /// </summary>
    /// <param name="request">The original request that caused the exception.</param>
    /// <param name="exception">The database update exception that occurred.</param>
    /// <param name="state">The request exception handler state.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A completed task.</returns>
    public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, 
            "Database update exception occurred while processing {RequestType}: {ExceptionType}",
            typeof(TRequest).Name, exception.GetType().Name);

        var errors = GetUserFriendlyErrors(exception);
        var failureResult = CreateFailureResult(errors);
        
        state.SetHandled(failureResult);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Creates a failure result using reflection to handle both generic and non-generic Result types.
    /// </summary>
    /// <param name="errors">The error messages to include in the result.</param>
    /// <returns>A failure result of type TResponse.</returns>
    private TResponse CreateFailureResult(string[] errors)
    {
        var responseType = typeof(TResponse);
        
        // Handle generic Result<T> types
        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = responseType.GetGenericArguments()[0];
            var failureMethod = typeof(Result<>)
                .MakeGenericType(valueType)
                .GetMethod("Failure", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string[]) }, null);

            if (failureMethod == null)
            {
                _logger.LogError("Could not find the 'Failure' method on Result<{ValueType}>", valueType.Name);
                throw new InvalidOperationException($"Could not find the 'Failure' method on Result<{valueType.Name}>.");
            }

            var resultObj = failureMethod.Invoke(null, new object[] { errors });
            if (resultObj == null)
            {
                _logger.LogError("The 'Failure' method returned null for Result<{ValueType}>", valueType.Name);
                throw new InvalidOperationException($"The 'Failure' method returned null for Result<{valueType.Name}>.");
            }

            return (TResponse)resultObj;
        }

        // Handle non-generic Result types
        return (TResponse)(object)Result.Failure(errors);
    }

    /// <summary>
    /// Maps specific database exceptions to user-friendly error messages.
    /// </summary>
    /// <param name="exception">The database update exception.</param>
    /// <returns>An array of user-friendly error messages.</returns>
    private string[] GetUserFriendlyErrors(DbUpdateException exception)
    {
        try
        {
            return exception switch
            {
                UniqueConstraintException uniqueEx => GetUniqueConstraintErrors(uniqueEx),
                CannotInsertNullException nullEx => GetCannotInsertNullErrors(nullEx),
                MaxLengthExceededException lengthEx => GetMaxLengthExceededErrors(lengthEx),
                NumericOverflowException overflowEx => GetNumericOverflowErrors(overflowEx),
                ReferenceConstraintException referenceEx => GetReferenceConstraintErrors(referenceEx),
                _ => GetGenericDatabaseErrors(exception)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing database exception messages");
            return GetGenericDatabaseErrors(exception);
        }
    }

    /// <summary>
    /// Gets user-friendly error messages for unique constraint violations.
    /// </summary>
    /// <param name="exception">The unique constraint exception.</param>
    /// <returns>An array of error messages.</returns>
    private string[] GetUniqueConstraintErrors(UniqueConstraintException exception)
    {
        var tableName = GetTableName(exception.SchemaQualifiedTableName);
        var properties = GetConstraintProperties(exception.ConstraintProperties);
        var constraintName = GetConstraintName(exception.ConstraintName);

        var errorMessage = !string.IsNullOrWhiteSpace(constraintName) && !string.IsNullOrWhiteSpace(properties)
            ? $"A record with the same {properties} already exists in {tableName}. Each {properties} must be unique."
            : $"A unique constraint violation occurred in {tableName}. Please ensure all values are unique.";

        return new[] { errorMessage };
    }

    /// <summary>
    /// Gets user-friendly error messages for null constraint violations.
    /// </summary>
    /// <param name="exception">The cannot insert null exception.</param>
    /// <returns>An array of error messages.</returns>
    private string[] GetCannotInsertNullErrors(CannotInsertNullException exception)
    {
        return new[] { "Some required information is missing. Please ensure all required fields are filled out." };
    }

    /// <summary>
    /// Gets user-friendly error messages for maximum length exceeded violations.
    /// </summary>
    /// <param name="exception">The max length exceeded exception.</param>
    /// <returns>An array of error messages.</returns>
    private string[] GetMaxLengthExceededErrors(MaxLengthExceededException exception)
    {
        return new[] { "Some input values are too long. Please reduce the length of the data entered." };
    }

    /// <summary>
    /// Gets user-friendly error messages for numeric overflow violations.
    /// </summary>
    /// <param name="exception">The numeric overflow exception.</param>
    /// <returns>An array of error messages.</returns>
    private string[] GetNumericOverflowErrors(NumericOverflowException exception)
    {
        return new[] { "One or more numbers are outside the allowed range. Please enter valid numbers within the specified limits." };
    }

    /// <summary>
    /// Gets user-friendly error messages for reference constraint violations.
    /// </summary>
    /// <param name="exception">The reference constraint exception.</param>
    /// <returns>An array of error messages.</returns>
    private string[] GetReferenceConstraintErrors(ReferenceConstraintException exception)
    {
        var tableName = GetTableName(exception.SchemaQualifiedTableName);

        return new[]
        {
            $"This operation cannot be completed because the record is linked to other data in {tableName}. " +
            "Please remove any related records first, or contact your administrator for assistance."
        };
    }

    /// <summary>
    /// Gets generic error messages for unhandled database exceptions.
    /// </summary>
    /// <param name="exception">The database update exception.</param>
    /// <returns>An array of error messages.</returns>
    private string[] GetGenericDatabaseErrors(DbUpdateException exception)
    {
        _logger.LogWarning("Unhandled database exception type: {ExceptionType}", exception.GetType().Name);
        
        return new[]
        {
            "A database error occurred while processing your request. Please try again, and contact support if the problem persists.",
            exception.GetBaseException().Message
        };
    }

    #region Helper Methods

    /// <summary>
    /// Extracts and formats the table name from a schema-qualified table name.
    /// </summary>
    /// <param name="schemaQualifiedTableName">The schema-qualified table name.</param>
    /// <returns>A formatted table name.</returns>
    private static string GetTableName(string? schemaQualifiedTableName)
    {
        if (string.IsNullOrWhiteSpace(schemaQualifiedTableName))
            return "the specified table";

        // Extract table name from schema.table format
        var parts = schemaQualifiedTableName.Split('.');
        var tableName = parts.Length > 1 ? parts[^1] : schemaQualifiedTableName;
        
        // Remove brackets and quotes if present
        tableName = tableName.Trim('[', ']', '"', '\'');
        
        return string.IsNullOrWhiteSpace(tableName) ? "the specified table" : tableName;
    }

    /// <summary>
    /// Formats constraint properties into a readable string.
    /// </summary>
    /// <param name="constraintProperties">The constraint properties.</param>
    /// <returns>A formatted string of properties.</returns>
    private static string GetConstraintProperties(IReadOnlyList<string>? constraintProperties)
    {
        if (constraintProperties == null || !constraintProperties.Any())
            return "specified properties";

        return constraintProperties.Count == 1 
            ? constraintProperties[0] 
            : string.Join(", ", constraintProperties);
    }

    /// <summary>
    /// Extracts and formats the constraint name.
    /// </summary>
    /// <param name="constraintName">The constraint name.</param>
    /// <returns>A formatted constraint name.</returns>
    private static string GetConstraintName(string? constraintName)
    {
        if (string.IsNullOrWhiteSpace(constraintName))
            return string.Empty;

        // Remove common prefixes and suffixes from constraint names
        var cleanName = constraintName
            .Replace("PK_", "")
            .Replace("FK_", "")
            .Replace("IX_", "")
            .Replace("UQ_", "")
            .Replace("UC_", "");
        
        return string.IsNullOrWhiteSpace(cleanName) ? string.Empty : cleanName;
    }

    /// <summary>
    /// Extracts and formats the column name.
    /// </summary>
    /// <param name="columnName">The column name.</param>
    /// <returns>A formatted column name.</returns>
    private static string GetColumnName(string? columnName)
    {
        if (string.IsNullOrWhiteSpace(columnName))
            return string.Empty;

        // Remove brackets and quotes if present
        var cleanName = columnName.Trim('[', ']', '"', '\'');
        
        return string.IsNullOrWhiteSpace(cleanName) ? string.Empty : cleanName;
    }

    #endregion
}
