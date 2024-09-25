using System.Text.RegularExpressions;
using EntityFramework.Exceptions.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Npgsql;

namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

public class DbExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : IRequest<Result<int>>
    where TResponse : Result<int>
    where TException : DbUpdateException
{
    private readonly ILogger<DbExceptionHandler<TRequest, TResponse, TException>> _logger;

    public DbExceptionHandler(ILogger<DbExceptionHandler<TRequest, TResponse, TException>> logger)
    {
        _logger = logger;
    }

    public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken)
    {
        // Log the exception details
        _logger.LogError("Database update exception occurred: {Message}", exception.GetBaseException().Message);

        // Set the handled result with user-friendly error messages
        state.SetHandled((TResponse)Result<int>.Failure(GetErrors(exception)));

        return Task.CompletedTask;
    }

    private static string[] GetErrors(DbUpdateException exception)
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
    private static string[] GetUniqueConstraintExceptionErrors(UniqueConstraintException exception)
    {
        var tableName = string.IsNullOrWhiteSpace(exception.SchemaQualifiedTableName) ? "unknown table" : exception.SchemaQualifiedTableName;
        var properties = exception.ConstraintProperties != null && exception.ConstraintProperties.Any()
            ? string.Join(", ", exception.ConstraintProperties)
            : "unknown properties";

        return new[]
        {
            $"A unique constraint violation occurred on constraint in table '{tableName}'. " +
            $"({properties}). Please ensure the values are unique."
        };
    }
    private static string[] GetCannotInsertNullExceptionErrors(CannotInsertNullException exception)
    {
        return new[]
        {
            "Some required information is missing. Please make sure all required fields are filled out."
        };
    }
    private static string[] GetMaxLengthExceededExceptionErrors(MaxLengthExceededException exception)
    {
        return new[]
        {
            "Some input is too long. Please shorten the data entered in the fields."
        };
    }
    private static string[] GetNumericOverflowExceptionErrors(NumericOverflowException exception)
    {
        return new[]
        {
           "A number you entered is too large or too small. Please enter a number within the allowed range."
        };
    }
    private static string[] GetReferenceConstraintExceptionErrors(ReferenceConstraintException exception)
    {
        var tableName = string.IsNullOrWhiteSpace(exception.SchemaQualifiedTableName) ? "unknown table" : exception.SchemaQualifiedTableName;
        return new[]
        {
            $"The operation failed because this record is linked to other records in {tableName}. " +
            $"Please remove any related records first"
        };
    }

}