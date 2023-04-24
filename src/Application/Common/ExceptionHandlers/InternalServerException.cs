namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
public class InternalServerException : ServerException
{
    public InternalServerException(string message)
        : base(message, System.Net.HttpStatusCode.InternalServerError)
    {
    }
}