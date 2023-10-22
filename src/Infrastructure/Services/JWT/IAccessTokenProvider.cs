using CleanArchitecture.Blazor.Domain.Features.Identity;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;

public interface IAccessTokenProvider
{
    string? AccessToken { get; }
    string? RefreshToken { get; }

    Task<ClaimsPrincipal> GetClaimsPrincipal();
    Task Login(ApplicationUser applicationUser);
    Task RemoveAuthDataFromStorage();
}