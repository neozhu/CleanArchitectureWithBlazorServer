using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
public class SimpleJwtOptions
{
    public const string Key = nameof(SimpleJwtOptions);

    public bool UseCookie { get; set; } = false;
    public CookieOptions CookieOptions { get; set; } = new CookieOptions()
    {
        HttpOnly = true,
        SameSite = SameSiteMode.Lax,
    };
    public string Issuer { get; set; } = "blazorserver";
    public string Audience { get; set; } = "blazorserver";

    /// <summary>
    /// Signing options used for signing access jwts
    /// </summary>
    public JwtSigningOptions AccessSigningOptions { get; set; } = new JwtSigningOptions()
    {
        Algorithm = SecurityAlgorithms.HmacSha256,
        ExpirationMinutes = 120
    };

    /// <summary>
    /// Signing options used for signing refresh jwts
    /// </summary>
    public JwtSigningOptions RefreshSigningOptions { get; set; } = new JwtSigningOptions()
    {
        Algorithm = SecurityAlgorithms.HmacSha256,
        ExpirationMinutes = 1440
    };

    /// <summary>
    /// Validation parameters used for verifying access jwts
    /// </summary>
    public TokenValidationParameters? AccessValidationParameters { get; set; }

    /// <summary>
    /// Validation parameters used for verifying refresh jwts
    /// </summary>
    public TokenValidationParameters? RefreshValidationParameters { get; set; }
}

