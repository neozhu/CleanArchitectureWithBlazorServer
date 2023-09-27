using System.IdentityModel.Tokens.Jwt;
using CleanArchitecture.Blazor.Application.Common.Configurations;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;

public class AccessTokenGenerator : IAccessTokenGenerator
{
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly SimpleJwtOptions _options;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    public AccessTokenGenerator(JwtSecurityTokenHandler tokenHandler, IOptions<SimpleJwtOptions> options, IServiceScopeFactory scopeFactory)
    {
        _tokenHandler = tokenHandler;
        _options = options.Value;
        var scope = scopeFactory.CreateScope();
        _userClaimsPrincipalFactory = scope.ServiceProvider.GetRequiredService<IUserClaimsPrincipalFactory<ApplicationUser>>();
    }

    public async Task<string> GenerateAccessToken(ApplicationUser user)
    {
        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var signingOptions = _options.AccessSigningOptions!;
        var credentials = new SigningCredentials(signingOptions.SigningKey, signingOptions.Algorithm);
        var header = new JwtHeader(credentials);
        var payload = new JwtPayload(
            _options.Issuer!,
            _options.Audience!,
            principal.Claims,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(signingOptions.ExpirationMinutes)
        );
        return _tokenHandler.WriteToken(new JwtSecurityToken(header, payload));
    }
}
