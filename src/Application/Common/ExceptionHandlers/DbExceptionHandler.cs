﻿using CleanArchitecture.Blazor.Application.Common.Models;
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
        if (exception.InnerException is SqlException sqlException)
        {
            // Handle specific SQL error numbers
            switch (sqlException.Number)
            {
                case 2627: // Unique constraint error
                    errors.Add("A record with the same unique value already exists. Duplicate records are not allowed.");
                    break;
                case 544: // Cannot insert explicit value for identity column
                    errors.Add("Cannot insert a value for the identity column because it is automatically generated by the system.");
                    break;
                case 547: // Constraint check violation (e.g., foreign key violation)
                    errors.Add("The operation failed because this record is referenced by another record. Please remove any related records first before deleting.");
                    break;
                case 2601: // Duplicated key row error
                    errors.Add("A duplicate key was detected. Please check the uniqueness of the data.");
                    break;
                case 201: // Procedure missing parameters
                    errors.Add("The stored procedure execution failed due to missing required parameters.");
                    break;
                case 2628: // String or binary data would be truncated
                    errors.Add("The data entered is too long for one or more fields. Please shorten the input.");
                    break;
                case 515: // Cannot insert the value NULL into column
                    errors.Add("A required field is missing. Please ensure that all required fields are filled.");
                    break;
                case 8134: // Divide by zero error encountered
                    errors.Add("A division by zero occurred in the SQL query. Please check your data or calculations.");
                    break;
                case 1205: // Deadlock victim
                    errors.Add("A deadlock occurred, and your request was chosen as the victim. Please try again.");
                    break;
                case 53: // SQL server not reachable
                    errors.Add("Cannot connect to the SQL server. Please verify the server is reachable and your connection settings are correct.");
                    break;
                case 18456: // Login failed
                    errors.Add("Login to the SQL server failed. Please check your credentials.");
                    break;
                case 1105: // Transaction log is full
                    errors.Add("The transaction log is full. Please contact your database administrator to resolve this issue.");
                    break;
                case 4060: // Cannot open database
                    errors.Add("Cannot open the database. Please ensure the database exists and you have permission to access it.");
                    break;
                default:
                    errors.Add($"An SQL error occurred. Error number: {sqlException.Number}. Message: {sqlException.Message}");
                    break;
            }
        }
        else
        {
            // Iterate over affected entries
            foreach (var result in exception.Entries)
            {
                errors.Add(
                    $"An error occurred while updating the entity of type {result.Entity.GetType().Name}. Entity state: {result.State}. Details: {exception.InnerException?.Message ?? exception.Message}");
            }
        }
        return errors.ToArray();
    }

}