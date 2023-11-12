using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
public interface IAccessTokenValidator
{
    Task<TokenValidationResult> ValidateTokenAsync(string token);
}
