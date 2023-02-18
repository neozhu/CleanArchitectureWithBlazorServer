namespace CleanArchitecture.Blazor.Application.Common.Exceptions;
public class InternalServerException : ServerException
{
    public InternalServerException(string message)
        : base(message, System.Net.HttpStatusCode.InternalServerError)
    {
    }
}