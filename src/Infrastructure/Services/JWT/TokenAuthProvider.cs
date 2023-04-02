using System.Security.Cryptography;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Application.Constants;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Services.MultiTenant;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
public class TokenAuthProvider
{
    private readonly string TokenKey = nameof(TokenKey);
    private readonly ProtectedLocalStorage _localStorage;
    private readonly NavigationManager _navigation;
    private readonly IIdentityService _identityService;
    private readonly ITenantProvider _tenantProvider;
    private readonly ICurrentUserService _currentUser;

    public TokenAuthProvider(ProtectedLocalStorage localStorage, NavigationManager navigation, IIdentityService identityService,
        ITenantProvider tenantProvider,
        ICurrentUserService currentUser)
    {
        _localStorage = localStorage;
        _navigation = navigation;
        _identityService = identityService;
        _tenantProvider = tenantProvider;
        _currentUser = currentUser;
    }
    public async Task GenerateJwt(ApplicationUser applicationUser)
    {
        var token = await _identityService.GenerateJwtAsync(applicationUser);
        await _localStorage.SetAsync(TokenKey, token);
        _tenantProvider.TenantId = applicationUser.TenantId;
        _tenantProvider.TenantName = applicationUser.TenantName;
        _currentUser.UserId = applicationUser.Id;
        _currentUser.UserName = applicationUser.UserName;
    }
    public async Task<ClaimsPrincipal> GetClaimsPrincipal()
    {
        try
        {
            var token = await _localStorage.GetAsync<string>(TokenKey);
            if (token.Success && !string.IsNullOrEmpty(token.Value))
            {
                var principal = await _identityService.ValidateToken(token.Value);
                if (principal?.Identity?.IsAuthenticated ?? false)
                {
                    _tenantProvider.TenantId = principal?.GetTenantId();
                    _tenantProvider.TenantName = principal?.GetTenantName();
                    _currentUser.UserId = principal?.GetUserId();
                    _currentUser.UserName = principal?.GetUserName();
                    return principal;
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
        await _localStorage.DeleteAsync(TokenKey);
        _navigation.NavigateTo("/", true);
    }
}
