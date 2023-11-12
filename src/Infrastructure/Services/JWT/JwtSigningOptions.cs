using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
/// <summary>
/// Signing options for Jwt
/// </summary>
public class JwtSigningOptions
{
    /// <summary>
    /// The signing key that is used to sign the content of generated tokens.
    /// </summary>
    public SecurityKey? SigningKey { get; set; } 

    /// <summary>
    /// To use symmetric HMAC signing and verification, the following algorithms may be used: 'HS256', 'HS384', 'HS512'.
    /// When an HMAC algorithm is chosen, the SigningKey setting will be used as both the signing key and the verifying key.
    /// To use asymmetric RSA signing and verification, the following algorithms may be used: 'RS256', 'RS384', 'RS512'. 
    /// When an RSA algorithm is chosen, the SigningKey setting must be set to an RsaSecurityKey that contains an RSA private key. 
    /// Likewise, the TokenValidationParammeters setting must be set to an RsaSecurityKey that contains an RSA public key.
    /// </summary>
    public string Algorithm { get; set; } = SecurityAlgorithms.HmacSha256;
    public int ExpirationMinutes { get; set; } = 120;
}
