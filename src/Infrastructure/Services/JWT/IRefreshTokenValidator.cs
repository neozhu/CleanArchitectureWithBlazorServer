using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
public interface IRefreshTokenValidator
{
    Task<TokenValidationResult> ValidateTokenAsync(string token);
}
