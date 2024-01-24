using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;

public interface IAccessTokenProvider
{
    string? AccessToken { get; }
    string? RefreshToken { get; }

    Task<ClaimsPrincipal> GetClaimsPrincipal();
    Task<ClaimsPrincipal> ParseClaimsFromJwt(string? accessToken);
    Task<string?> Login(ApplicationUser applicationUser);
    ValueTask RemoveAuthDataFromStorage();
}