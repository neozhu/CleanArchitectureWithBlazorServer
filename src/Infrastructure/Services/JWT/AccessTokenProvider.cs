using System.Security.Cryptography;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
public class AccessTokenProvider
{
    private readonly string _tokenKey = nameof(_tokenKey);
    private readonly string _refreshTokenKey = nameof(_refreshTokenKey);
    private readonly ProtectedLocalStorage _localStorage;
    private readonly ILoginService _loginService;
    private readonly ITokenValidator _tokenValidator;
    private readonly ITenantProvider _tenantProvider;
    private readonly ICurrentUserService _currentUser;
    public string? AccessToken { get; private set; }
    public string? RefreshToken { get; private set; }

    public AccessTokenProvider(ProtectedLocalStorage localStorage, ILoginService loginService, ITokenValidator tokenValidator,
        ITenantProvider tenantProvider,
        ICurrentUserService currentUser)
    {
        _localStorage = localStorage;
        _loginService = loginService;
        _tokenValidator = tokenValidator;
        _tenantProvider = tenantProvider;
        _currentUser = currentUser;
    }
    public async Task Login(ApplicationUser applicationUser)
    {
        var token = await _loginService.LoginAsync(applicationUser);
        await _localStorage.SetAsync(_tokenKey, token.AccessToken??"");
        await _localStorage.SetAsync(_refreshTokenKey, token.RefreshToken??"");
        AccessToken = token.AccessToken;
        RefreshToken = token.RefreshToken;
        _tenantProvider.TenantId = applicationUser.TenantId;
        _tenantProvider.TenantName = applicationUser.TenantName;
        _currentUser.UserId = applicationUser.Id;
        _currentUser.UserName = applicationUser.UserName;
        _currentUser.TenantId = applicationUser.TenantId;
        _currentUser.TenantName = applicationUser.TenantName;

    }
    public async Task<ClaimsPrincipal> GetClaimsPrincipal()
    {
        try
        {
            var token = await _localStorage.GetAsync<string>(_tokenKey);
            if (token.Success && !string.IsNullOrEmpty(token.Value))
            {
                AccessToken = token.Value;
                var refreshToken = await _localStorage.GetAsync<string>(_refreshTokenKey);
                RefreshToken = refreshToken.Value;
                var result = await _tokenValidator.ValidateTokenAsync(token.Value);
                if (result.IsValid)
                {
                    var principal=new ClaimsPrincipal(result.ClaimsIdentity);
                    _tenantProvider.TenantId = principal.GetTenantId();
                    _tenantProvider.TenantName = principal.GetTenantName();
                    _currentUser.UserId = principal.GetUserId();
                    _currentUser.UserName = principal.GetUserName();
                    _currentUser.TenantId = principal.GetTenantId();
                    _currentUser.TenantName = principal.GetTenantId();
                    return principal!;
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


    public async Task RemoveAuthDataFromStorage()
    {
        await _localStorage.DeleteAsync(_tokenKey);
    }
}
