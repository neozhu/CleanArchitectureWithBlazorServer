using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
/// <summary>
/// Use this class to validate refresh tokens
/// </summary>
public class RefreshTokenValidator : IRefreshTokenValidator
{
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly TokenValidationParameters _validationParameters;

    public RefreshTokenValidator(JwtSecurityTokenHandler tokenHandler, IOptions<SimpleJwtOptions> options)
    {
        _tokenHandler = tokenHandler;
        _validationParameters = options.Value.RefreshValidationParameters!;

    }

    /// <summary>
    /// Validate a refresh token
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<TokenValidationResult> ValidateTokenAsync(string token)
    {

        // if not exists: return invalid validation result.
        if (token is null)
        {
            return new TokenValidationResult
            {
                IsValid = false,
                Exception = new RefreshTokenNotFoundException("Refresh token might be invalid or expired"),
            };
        }

        // if jwt is invalid: delete token from db? and return validation result.
        var result = await _tokenHandler.ValidateTokenAsync(token, _validationParameters);
        if (!result.IsValid)
        {
            // delete token from db
            // ...
            return result;
        }

        return result;
    }
}
