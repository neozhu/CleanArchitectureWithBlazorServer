using EntityFramework.Exceptions.Common;
using System.Collections.Concurrent;

namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

/// <summary>
/// Handles database update exceptions and converts them into Result or Result&lt;T&gt; responses.
/// Provides user-friendly error messages for various database constraint violations.
/// </summary>
public sealed class DbExceptionHandler<TRequest, TResponse, TException> :
    IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : IRequest<TResponse>
    where TResponse : IResult
    where TException : DbUpdateException
{
    // Cache compiled factories per TResponse type to avoid repeated reflection
    private static readonly ConcurrentDictionary<Type, Func<string[], object>> FailureFactoryCache = new();

    // Common constraint-name prefixes to strip
    private static readonly string[] ConstraintPrefixes = ["PK_", "FK_", "IX_", "UQ_", "UC_"];
    public Task Handle(
        TRequest request,
        TException exception,
        RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken)
    {
        var errors = GetUserFriendlyErrors(exception);
        var failureResult = CreateFailureResult(errors);

        state.SetHandled(failureResult);
        return Task.CompletedTask;
    }

    private TResponse CreateFailureResult(string[] errors)
    {
        var factory = FailureFactoryCache.GetOrAdd(typeof(TResponse), CreateFailureFactory);
        return (TResponse)factory(errors);
    }

    /// <summary>
    /// Build a cached delegate to create failure results without repeated reflection.
    /// </summary>
    private static Func<string[], object> CreateFailureFactory(Type responseType)
    {
        var errorsParam = Expression.Parameter(typeof(string[]), "errors");

        if (responseType.IsGenericType &&
            responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = responseType.GetGenericArguments()[0];
            var genericResultType = typeof(Result<>).MakeGenericType(valueType);

            var failureMethod = genericResultType.GetMethod(
                "Failure",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: [typeof(string[])],
                modifiers: null)
                ?? throw new InvalidOperationException(
                    $"Could not find the 'Failure' method on Result<{valueType.Name}>.");

            var call = Expression.Call(null, failureMethod, errorsParam);
            var cast = Expression.Convert(call, typeof(object));
            return Expression.Lambda<Func<string[], object>>(cast, errorsParam).Compile();
        }
        else
        {
            var failureMethod = typeof(Result).GetMethod(
                "Failure",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: [typeof(string[])],
                modifiers: null)
                ?? throw new InvalidOperationException("Could not find the 'Failure' method on Result.");

            var call = Expression.Call(null, failureMethod, errorsParam);
            var cast = Expression.Convert(call, typeof(object));
            return Expression.Lambda<Func<string[], object>>(cast, errorsParam).Compile();
        }
    }

    /// <summary>
    /// Maps specific database exceptions to user-friendly error messages.
    /// </summary>
    private string[] GetUserFriendlyErrors(DbUpdateException exception) =>
        exception switch
        {
            UniqueConstraintException uniqueEx => GetUniqueConstraintErrors(uniqueEx),
            CannotInsertNullException => ["Required fields are missing. Please fill in all mandatory information."],
            MaxLengthExceededException => ["Input data exceeds maximum length. Please reduce the size of your entries."],
            NumericOverflowException => ["Numeric values are outside the valid range. Please enter appropriate numbers."],
            ReferenceConstraintException referenceEx => GetReferenceConstraintErrors(referenceEx),
            _ => GetGenericDatabaseErrors(exception)
        };

    /// <summary>
    /// Gets user-friendly error messages for unique constraint violations.
    /// </summary>
    private static string[] GetUniqueConstraintErrors(UniqueConstraintException exception)
    {
        var tableName = GetTableName(exception.SchemaQualifiedTableName);
        var propertiesStr = GetConstraintProperties(exception.ConstraintProperties);

        var message = !string.IsNullOrWhiteSpace(propertiesStr) && propertiesStr != "specified properties"
            ? $"A record with the same {propertiesStr} already exists. Each {propertiesStr} must be unique."
            : "A duplicate record was found. Please ensure all values are unique.";

        return [message];
    }

    /// <summary>
    /// Gets user-friendly error messages for reference constraint violations.
    /// </summary>
    private static string[] GetReferenceConstraintErrors(ReferenceConstraintException exception)
    {
        return ["Cannot complete this operation because the record has dependent data. Please remove related records first."];
    }

    /// <summary>
    /// Gets generic error messages for unhandled database exceptions.
    /// </summary>
    private string[] GetGenericDatabaseErrors(DbUpdateException exception)
    {
        return ["A database error occurred. Please try again or contact support if the issue persists."];
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
            return "table";

        // Extract table name from schema.table format
        var parts = schemaQualifiedTableName.Split('.');
        var tableName = parts.Length > 1 ? parts[^1] : schemaQualifiedTableName;
        
        // Remove brackets and quotes if present
        tableName = tableName.Trim('[', ']', '"', '\'');
        
        return string.IsNullOrWhiteSpace(tableName) ? "table" : tableName;
    }

    /// <summary>
    /// Formats constraint properties into a readable string.
    /// </summary>
    /// <param name="constraintProperties">The constraint properties.</param>
    /// <returns>A formatted string of properties.</returns>
    private static string GetConstraintProperties(IReadOnlyList<string>? constraintProperties)
    {
        if (constraintProperties is null or { Count: 0 })
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

        // Remove common prefixes from constraint names
        var clean = constraintName.AsSpan();
        foreach (var prefix in ConstraintPrefixes)
        {
            if (clean.StartsWith(prefix))
            {
                clean = clean[prefix.Length..];
                break;
            }
        }

        return clean.IsEmpty ? string.Empty : clean.ToString();
    }

    #endregion
}
