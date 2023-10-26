namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;

public class TokenGeneratorService : ITokenGeneratorService
{
    protected readonly IAccessTokenGenerator _accessTokenGenerator;
    protected readonly IRefreshTokenGenerator _refreshTokenGenerator;

    public TokenGeneratorService(IAccessTokenGenerator accessTokenGenerator, IRefreshTokenGenerator refreshTokenGenerator)
    {
        _accessTokenGenerator = accessTokenGenerator;
        _refreshTokenGenerator = refreshTokenGenerator;
    }

    public string GenerateAccessToken(ClaimsPrincipal user)
    {
        return _accessTokenGenerator.GenerateAccessToken(user);
    }

    public string GenerateRefreshToken(ClaimsPrincipal user)
    {
        return _refreshTokenGenerator.GenerateRefreshToken(user);
    }
}
