namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
public class RefreshTokenNotFoundException : Exception
{
    public RefreshTokenNotFoundException()
    {
    }

    public RefreshTokenNotFoundException(string message)
        : base(message)
    {
    }

    public RefreshTokenNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
