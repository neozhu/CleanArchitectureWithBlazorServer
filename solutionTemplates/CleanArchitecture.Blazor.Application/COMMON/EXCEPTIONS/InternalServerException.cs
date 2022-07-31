namespace CleanArchitecture.Blazor.$safeprojectname$.Common.Exceptions;
public class InternalServerException : CustomException
{
    public InternalServerException(string message, List<string>? errors = default)
        : base(message, errors, System.Net.HttpStatusCode.InternalServerError)
    {
    }
}