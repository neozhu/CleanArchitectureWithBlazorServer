using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
public class AccessTokenProvider : IAccessTokenProvider
{
    private readonly string _tokenKey = nameof(_tokenKey);
    private readonly string _refreshTokenKey = nameof(_refreshTokenKey);
    private readonly ProtectedLocalStorage _localStorage;
    private readonly ILoginService _loginService;
    private readonly IAccessTokenValidator _tokenValidator;
    private readonly IRefreshTokenValidator _refreshTokenValidator;
    private readonly IAccessTokenGenerator _tokenGenerator;
    private readonly ITenantProvider _tenantProvider;
    private readonly ICurrentUserService _currentUser;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    public string? AccessToken { get; private set; }
    public string? RefreshToken { get; private set; }

    public AccessTokenProvider(IServiceScopeFactory scopeFactory, 
        ProtectedLocalStorage localStorage,
        ILoginService loginService, 
        IAccessTokenValidator tokenValidator, 
        IRefreshTokenValidator refreshTokenValidator,
        IAccessTokenGenerator tokenGenerator,
        ITenantProvider tenantProvider,
        ICurrentUserService currentUser)
    {
        var scope = scopeFactory.CreateScope();
        _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        _userClaimsPrincipalFactory = scope.ServiceProvider.GetRequiredService<IUserClaimsPrincipalFactory<ApplicationUser>>();
        _localStorage = localStorage;
        _loginService = loginService;
        _tokenValidator = tokenValidator;
        _refreshTokenValidator = refreshTokenValidator;
        _tokenGenerator = tokenGenerator;
        _tenantProvider = tenantProvider;
        _currentUser = currentUser;
    }
    public async Task<string?> Login(ApplicationUser applicationUser)
    {
        var principal = await _userClaimsPrincipalFactory.CreateAsync(applicationUser);
        var token = await _loginService.LoginAsync(principal);
        await  _localStorage.SetAsync(_tokenKey, token);
        AccessToken = token.AccessToken;
        RefreshToken = token.RefreshToken;
        SetUserPropertiesFromClaimsPrincipal(principal);
        return AccessToken;
    }
    public async Task<ClaimsPrincipal> ParseClaimsFromJwt(string? accessToken)
    {
        if(string.IsNullOrEmpty(accessToken)) return  new ClaimsPrincipal(new ClaimsIdentity());
        var validationResult = await _tokenValidator.ValidateTokenAsync(accessToken);
        if (validationResult.IsValid)
        {
            return SetUserPropertiesFromClaimsPrincipal(new ClaimsPrincipal(validationResult.ClaimsIdentity));
        }
        return new ClaimsPrincipal(new ClaimsIdentity());
    }
    public async Task<ClaimsPrincipal> GetClaimsPrincipal()
    {
        try
        {
            var token = await _localStorage.GetAsync<AuthenticatedUserResponse>(_tokenKey);
            if (token.Success && token.Value is not null)
            {
                AccessToken = token.Value.AccessToken;
                RefreshToken = token.Value.RefreshToken;
                var validationResult = await _tokenValidator.ValidateTokenAsync(AccessToken!);
                if (validationResult.IsValid)
                {
                    
                    return SetUserPropertiesFromClaimsPrincipal(new ClaimsPrincipal(validationResult.ClaimsIdentity));
                }
                else
                {
                    var validationRefreshResult = await _refreshTokenValidator.ValidateTokenAsync(RefreshToken!);
                    if (validationRefreshResult.IsValid)
                    {

                        return SetUserPropertiesFromClaimsPrincipal(new ClaimsPrincipal(validationRefreshResult.ClaimsIdentity));
                    }

                }
            }
        }
        catch (CryptographicException)
        {
            await RemoveAuthDataFromStorage();
        }
        catch (Exception)
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }
        return new ClaimsPrincipal(new ClaimsIdentity());
    }

    private ClaimsPrincipal SetUserPropertiesFromClaimsPrincipal(ClaimsPrincipal principal)
    {
        _tenantProvider.TenantId = principal.GetTenantId();
        _tenantProvider.TenantName = principal.GetTenantName();
        _currentUser.UserId = principal.GetUserId();
        _currentUser.UserName = principal.GetUserName();
        _currentUser.TenantId = principal.GetTenantId();
        _currentUser.TenantName = principal.GetTenantName();  // This seems to be an error in original code. Fixing it here.
        return principal;
    }

     
    public ValueTask RemoveAuthDataFromStorage() => _localStorage.DeleteAsync(_tokenKey);

    public async Task<string> Refresh(string refreshToken)
    {
        TokenValidationResult validationResult = await _tokenValidator.ValidateTokenAsync(refreshToken!);
        if (!validationResult.IsValid)
        {
            throw validationResult.Exception;
        }
        JwtSecurityToken? jwt = validationResult.SecurityToken as JwtSecurityToken;
        string userId = jwt!.Claims.First(claim => claim.Type == "id").Value;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new Exception($"no found user by userId:{userId}");
        }
        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        string accessToken = _tokenGenerator.GenerateAccessToken(principal);
        return accessToken;
    }
}
