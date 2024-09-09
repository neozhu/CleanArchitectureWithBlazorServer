using CleanArchitecture.Blazor.Application.Common.Models;
using Microsoft.Data.SqlClient;

namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

public class
    DbExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
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
        state.SetHandled((TResponse)Result<int>.Failure(GetErrors(exception)));
        return Task.CompletedTask;
    }

    private static string[] GetErrors(DbUpdateException exception)
    {
        IList<string> errors = new List<string>();

        // Check if InnerException is a SqlException and cast it
        if (exception.InnerException is SqlException sqlException)
        {
            // Handle specific SQL error numbers
            switch (sqlException.Number)
            {
                case 2627: // Unique constraint error
                    errors.Add("A unique constraint error occurred. Duplicate records cannot be inserted.");
                    break;
                case 544: // Cannot insert explicit value for identity column
                    errors.Add("Cannot insert explicit value for identity column when IDENTITY_INSERT is set to OFF.");
                    break;
                case 547: // Constraint check violation
                    errors.Add("A constraint check violation occurred while updating the record(s).");
                    break;
                case 2601: // Duplicated key row error
                    errors.Add("A duplicate key error occurred while updating the record(s).");
                    break;
                case 201: // Procedure missing parameters
                    errors.Add("A missing parameter caused an error while creating the record(s).");
                    break;
                case 2628: // String or binary data would be truncated
                    errors.Add("Data too long for one or more fields. The provided string or binary data would be truncated.");
                    break;
                    break;
                default:
                    errors.Add($"An SQL error occurred. Error number: {sqlException.Number}, Message: {sqlException.Message}");
                    break;
            }
        }

        // Iterate over affected entries
        foreach (var result in exception.Entries)
        {
            errors.Add(
                $"An error occurred while updating the entity of type {result.Entity.GetType().Name}. Entity state: {result.State}. Details: {exception.InnerException?.Message ?? exception.Message}");
        }

        return errors.ToArray();
    }

}