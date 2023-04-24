using Microsoft.Data.SqlClient;

namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

public class
    DbExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : IRequest<Result>
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
        TResponse response = Activator.CreateInstance<TResponse>();
        if (response is Result result)
        {
            result.Succeeded = false;
            result.Errors = GetErrors(exception);
            state.SetHandled(response);
        }

        return Task.CompletedTask;
    }

    private static string[] GetErrors(DbUpdateException exception)
    {
        IList<string> errors = new List<string>();
        if (exception.InnerException != null
            && exception.InnerException != null
            && exception.InnerException is SqlException sqlException
           )
        {
            switch (sqlException.Number)
            {
                case 2627: // Unique constraint error
                    errors.Add(
                        "A Unique Constraint Error Has Occured While Updating the record! Duplicate Record cannot be inserted in the System.");
                    break;
                case 544
                    : // Cannot insert explicit value for identity column in table 'Departments' when IDENTITY_INSERT is set to OFF
                    errors.Add(
                        "Cannot insert explicit value for identity column in the system when the id is set to OFF");
                    break;
                case 547: // Constraint check violation, Conflict in the database
                    errors.Add("A Constraint Check violation Error Has Occured While Updating the record(s)!");
                    break;
                case 2601
                    : // Duplicated key row error // Constraint violation exception // A custom exception of yours for concurrency issues           
                    errors.Add("A Duplicate Key Error Has Occured While Updating the record(s)!");
                    break;
                case 201: // Procedure missing parameters            
                    errors.Add("A Missing Parameter has led to Error  While Creating the record(s)!");
                    break;
            }
        }


        return errors.ToArray();
    }
}