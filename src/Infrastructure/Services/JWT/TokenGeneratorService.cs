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

    public Task<string> GenerateAccessToken(ApplicationUser user)
    {
        return _accessTokenGenerator.GenerateAccessToken(user);
    }

    public Task<string> GenerateRefreshToken(ApplicationUser user)
    {
        return _refreshTokenGenerator.GenerateRefreshToken(user);
    }
}
