namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
public class ConflictException : ServerException
{
    public ConflictException(string message)
        : base(message, System.Net.HttpStatusCode.Conflict)
    {
    }
}
