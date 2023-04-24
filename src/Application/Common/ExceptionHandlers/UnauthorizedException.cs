namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
public class UnauthorizedException : ServerException
{
    public UnauthorizedException(string message)
       : base(message, System.Net.HttpStatusCode.Unauthorized)
    {
    }
}

