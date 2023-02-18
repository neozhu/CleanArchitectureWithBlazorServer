namespace CleanArchitecture.Blazor.Application.Common.Exceptions;
public class ConflictException : ServerException
{
    public ConflictException(string message)
        : base(message, System.Net.HttpStatusCode.Conflict)
    {
    }
}
