using CleanArchitecture.Blazor.Domain.Features.Identity;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;

public interface IAccessTokenGenerator
{
    string GenerateAccessToken(ClaimsPrincipal user);
}
