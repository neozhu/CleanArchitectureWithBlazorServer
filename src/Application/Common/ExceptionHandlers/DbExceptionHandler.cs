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
        var baseException = exception.GetBaseException();

        return baseException switch
        {
            SqlException sqlException => GetSqlExceptionErrors(sqlException),
            PostgresException pgException => GetPostgresExceptionErrors(pgException),
            SqliteException sqliteException => GetSqliteExceptionErrors(sqliteException),
            _ => new[] { baseException.Message }
        };
    }

    private static string[] GetSqlExceptionErrors(SqlException sqlException)
    {
        var errors = new List<string>();

        switch (sqlException.Number)
        {
            case 2627: // Unique constraint error
            case 2601: // Duplicated key row error
                errors.Add("A record with the same unique value already exists. Duplicate records are not allowed.");
                break;
            case 515: // Cannot insert the value NULL into column
                errors.Add("A required field is missing. Please ensure that all required fields are filled.");
                break;
            case 547: // Constraint check violation (e.g., foreign key violation)
                errors.Add("The operation failed because this record is referenced by another record. Please remove any related records first before deleting.");
                break;
            case 2628: // String or binary data would be truncated
                errors.Add("The data entered is too long for one or more fields. Please shorten the input.");
                break;
            case 1205: // Deadlock victim
                errors.Add("A deadlock occurred, and your request was chosen as the victim. Please try again.");
                break;
            case 4060: // Cannot open database
            case 18456: // Login failed
                errors.Add("Cannot connect to the SQL server. Please verify your credentials and network connection.");
                break;
            default:
                errors.Add($"An SQL error occurred. Error number: {sqlException.Number}. Message: {sqlException.Message}");
                break;
        }

        return errors.ToArray();
    }

    private static string[] GetPostgresExceptionErrors(PostgresException pgException)
    {
        var errors = new List<string>();

        switch (pgException.SqlState)
        {
            case PostgresErrorCodes.UniqueViolation:
                errors.Add("A record with the same unique value already exists. Duplicate records are not allowed.");
                break;
            case PostgresErrorCodes.NotNullViolation:
                errors.Add("A required field is missing. Please ensure that all required fields are filled.");
                break;
            case PostgresErrorCodes.ForeignKeyViolation:
                errors.Add("The operation failed because this record is referenced by another record. Please remove any related records first before deleting.");
                break;
            case PostgresErrorCodes.StringDataRightTruncation:
                errors.Add("The data entered is too long for one or more fields. Please shorten the input.");
                break;
            case PostgresErrorCodes.SerializationFailure:
                errors.Add("A serialization failure occurred, possibly due to a deadlock. Please try again.");
                break;
            case PostgresErrorCodes.ConnectionException:
                errors.Add("Cannot connect to the PostgreSQL server. Please verify your credentials and network connection.");
                break;
            default:
                errors.Add($"A PostgreSQL error occurred. Error code: {pgException.SqlState}. Message: {pgException.Message}");
                break;
        }

        return errors.ToArray();
    }

    private static string[] GetSqliteExceptionErrors(SqliteException sqliteException)
    {
        var errors = new List<string>();

        switch (sqliteException.SqliteErrorCode)
        {
            case 19: // SQLITE_CONSTRAINT
                switch (sqliteException.SqliteExtendedErrorCode)
                {
                    case 1555: // SQLITE_CONSTRAINT_PRIMARYKEY
                    case 2067: // SQLITE_CONSTRAINT_UNIQUE
                        errors.Add("A record with the same unique value already exists. Duplicate records are not allowed.");
                        break;
                    case 1299: // SQLITE_CONSTRAINT_NOTNULL
                        errors.Add("A required field is missing. Please ensure that all required fields are filled.");
                        break;
                    case 787: // SQLITE_CONSTRAINT_FOREIGNKEY
                        errors.Add("The operation failed because this record is referenced by another record. Please remove any related records first before deleting.");
                        break;
                    default:
                        errors.Add("A constraint violation occurred in the database operation.");
                        break;
                }
                break;
            case 8: // SQLITE_READONLY
                errors.Add("The database is in read-only mode. Write operations are not allowed.");
                break;
            default:
                errors.Add($"A SQLite error occurred. Error code: {sqliteException.SqliteErrorCode}. Message: {sqliteException.Message}");
                break;
        }

        return errors.ToArray();
    }
}