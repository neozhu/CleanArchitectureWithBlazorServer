namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
/// <summary>
/// Use this class to log a user in.
/// </summary>
/// <typeparam name="TUser"></typeparam>
public class JwtLoginService : ILoginService
{
    private readonly ITokenGeneratorService tokenGenerator;

    public JwtLoginService(ITokenGeneratorService tokenGenerator)
    {
        this.tokenGenerator = tokenGenerator;
    }

    /// <summary>
    /// Use this method to get an access Token and a refresh Token for the given TUser
    /// </summary>
    /// <param name="user"></param>
    /// <returns>An instance of <see cref="AuthenticatedUserResponse"/>, containing an access Token and a refresh Token</returns>
    public Task<AuthenticatedUserResponse> LoginAsync(ClaimsPrincipal user)
    {
        var accessToken = tokenGenerator.GenerateAccessToken(user);
        var refreshToken = tokenGenerator.GenerateRefreshToken(user);
        return Task.FromResult(new AuthenticatedUserResponse()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        });
    }
}

