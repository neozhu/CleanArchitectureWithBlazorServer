using CleanArchitecture.Blazor.Domain.Features.Identity;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
public interface IRefreshTokenGenerator
{
    string GenerateRefreshToken(ClaimsPrincipal user);
}
