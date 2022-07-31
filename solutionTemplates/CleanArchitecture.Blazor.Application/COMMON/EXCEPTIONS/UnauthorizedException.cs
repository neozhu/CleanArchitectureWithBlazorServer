namespace CleanArchitecture.Blazor.$safeprojectname$.Common.Exceptions;
public class UnauthorizedException : CustomException
{
    public UnauthorizedException(string message)
       : base(message, null, System.Net.HttpStatusCode.Unauthorized)
    {
    }
}

