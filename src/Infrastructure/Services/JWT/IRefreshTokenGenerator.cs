namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
public interface IRefreshTokenGenerator
{
    Task<string> GenerateRefreshToken(ApplicationUser user);
}
