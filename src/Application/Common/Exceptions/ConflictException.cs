namespace CleanArchitecture.Blazor.Application.Common.Exceptions;
public class ConflictException : CustomException
{
    public ConflictException(string message)
        : base(message, null, System.Net.HttpStatusCode.Conflict)
    {
    }
}
