using System.IdentityModel.Tokens.Jwt;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Extensions;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;

public class DefaultAuthenticator : IAuthenticator
{
    private readonly IAccessTokenGenerator _tokenGenerator;
    private readonly IAccessTokenValidator _tokenValidator;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    //private readonly UserManager<ApplicationUser> _userManager;
    private readonly CustomUserManager _userManager;

    public DefaultAuthenticator(IServiceScopeFactory scopeFactory, IAccessTokenValidator tokenValidator,
        IAccessTokenGenerator tokenGenerator)
    {
        var scope = scopeFactory.CreateScope();
        _userManager = scope.ServiceProvider.GetRequiredService<CustomUserManager>();
        _userClaimsPrincipalFactory =
                scope.ServiceProvider.GetRequiredService<IUserClaimsPrincipalFactory<ApplicationUser>>();
        _tokenValidator = tokenValidator;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<ApplicationUser?> Authenticate(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user == null) return null;

        var correctPassword = await _userManager.CheckPasswordAsync(user, password);

        if (!correctPassword) return null;

        return user;
    }

    public async Task<string> Refresh(string refreshToken)
    {
        var validationResult = await _tokenValidator.ValidateTokenAsync(refreshToken!);
        if (!validationResult.IsValid) return string.Empty;
        var jwt = validationResult.SecurityToken as JwtSecurityToken;
        var userId = jwt!.Claims.First(claim => claim.Type == "id").Value;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return string.Empty;
        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var accessToken = _tokenGenerator.GenerateAccessToken(principal);
        return accessToken;
    }
}