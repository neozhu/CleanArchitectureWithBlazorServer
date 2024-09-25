using System.Text.RegularExpressions;
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
                errors.Add($"A record with the same unique value already exists. Duplicate value: '{ExtractDuplicateKeyValue(sqlException.Message)}'. Duplicate records are not allowed.");
                break;
            case 515: // Cannot insert the value NULL into column
                var (t1, c1) = ExtractNullColumnInfo(sqlException.Message);
                errors.Add($"A required field '{c1}' in table '{t1}' is missing. Please ensure that all required fields are filled.");
                break;
            case 547: // Constraint check violation (e.g., foreign key violation)
                var (t2, c2) = ExtractConstraintInfo(sqlException.Message);
                errors.Add($"The operation failed because this record is referenced by another record in table '{t2}' (column: '{c2}'). Please remove any related records first before deleting.");
                break;
            case 2628: // String or binary data would be truncated
                var (columnName, truncatedValue) = ExtractTruncatedColumnInfo(sqlException.Message);
                errors.Add($"The data entered for column '{columnName}' is too long. Truncated value: '{truncatedValue}'. Please shorten the input.");
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
                var (t1, c1) = ExtractPGNullColumnInfo(pgException.Message);
                errors.Add($"A required field '{c1}' in table '{t1}' is missing. Please ensure that all required fields are filled.");
                break;
            case PostgresErrorCodes.ForeignKeyViolation:
                var (t2, constraint) = ExtractForeignKeyViolationInfo(pgException.Message);
                errors.Add($"The operation failed because this record is referenced by another record in table '{t2}' (constraint: '{constraint}'). Please remove any related records first before deleting.");
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


    private static string ExtractDuplicateKeyValue(string message)
    {
        var match = Regex.Match(message, @"duplicate key value is \(([^)]+)\)");
        return match.Success ? match.Groups[1].Value : "unknown value";
    }
    private static (string columnName, string truncatedValue) ExtractTruncatedColumnInfo(string message)
    {
        var columnMatch = Regex.Match(message, @"column '([^']+)'");
        var valueMatch = Regex.Match(message, @"Truncated value: '([^']+)'");

        string columnName = columnMatch.Success ? columnMatch.Groups[1].Value : "unknown column";
        string truncatedValue = valueMatch.Success ? valueMatch.Groups[1].Value : "unknown value";

        return (columnName, truncatedValue);
    }
    private static ( string table, string column) ExtractConstraintInfo(string message)
    {
        var tableMatch = Regex.Match(message, @"table ""([^""]+)""");
        var columnMatch = Regex.Match(message, @"column '([^']+)'");

        string table = tableMatch.Success ? tableMatch.Groups[1].Value : "unknown table";
        string column = columnMatch.Success ? columnMatch.Groups[1].Value : "unknown column";

        return (table, column);
    }
    private static (string table, string column) ExtractNullColumnInfo(string message)
    {
        var tableMatch = Regex.Match(message, @"table '([^']+)'");
        var columnMatch = Regex.Match(message, @"column '([^']+)'");

        string table = tableMatch.Success ? tableMatch.Groups[1].Value : "unknown table";
        string column = columnMatch.Success ? columnMatch.Groups[1].Value : "unknown column";

        return (table, column);
    }

    private static (string table, string column) ExtractPGNullColumnInfo(string message)
    {
        var tableMatch = Regex.Match(message, @"column ""([^""]+)"" of relation ""([^""]+)""");

        string column = tableMatch.Success ? tableMatch.Groups[1].Value : "unknown column";
        string table = tableMatch.Success ? tableMatch.Groups[2].Value : "unknown table";

        return (table, column);
    }
    private static (string table, string constraint) ExtractForeignKeyViolationInfo(string message)
    {
        var tableMatch = Regex.Match(message, @"table ""([^""]+)""");
        var constraintMatch = Regex.Match(message, @"constraint ""([^""]+)""");

        string table = tableMatch.Success ? tableMatch.Groups[1].Value : "unknown table";
        string constraint = constraintMatch.Success ? constraintMatch.Groups[1].Value : "unknown constraint";

        return (table, constraint);
    }
}