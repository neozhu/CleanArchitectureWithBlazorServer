using System.Security.Cryptography;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Domain.Features.Identity;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
public class AccessTokenProvider : IAccessTokenProvider
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
        await Task.WhenAll(_localStorage.SetAsync(_tokenKey, token.AccessToken ?? "").AsTask(),
                    _localStorage.SetAsync(_refreshTokenKey, token.RefreshToken ?? "").AsTask()
                    );
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
            var results = await Task.WhenAll(_localStorage.GetAsync<string>(_tokenKey).AsTask(),
                                             _localStorage.GetAsync<string>(_refreshTokenKey).AsTask()
                                            );

            if (results[0].Success && !string.IsNullOrEmpty(results[0].Value))
            {
                AccessToken = results[0].Value;
                var refreshToken = await _localStorage.GetAsync<string>(_refreshTokenKey);
                RefreshToken = results[1].Value;
                var result = await _tokenValidator.ValidateTokenAsync(AccessToken!);
                if (result.IsValid)
                {
                    var principal = new ClaimsPrincipal(result.ClaimsIdentity);
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


    public Task RemoveAuthDataFromStorage() => Task.WhenAll(_localStorage.DeleteAsync(_tokenKey).AsTask(),
                  _localStorage.DeleteAsync(_refreshTokenKey).AsTask());
     
}
