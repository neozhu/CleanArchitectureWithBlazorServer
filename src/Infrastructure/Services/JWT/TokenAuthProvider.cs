using System.Security.Cryptography;
using CleanArchitecture.Blazor.Application.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
public class TokenAuthProvider
{
    private readonly string TokenKey = nameof(TokenKey);
    private readonly ProtectedLocalStorage _localStorage;
    private readonly NavigationManager _navigation;
    private readonly IIdentityService _identityService;

    public TokenAuthProvider(ProtectedLocalStorage localStorage, NavigationManager navigation, IIdentityService identityService)
    {
        _localStorage = localStorage;
        _navigation = navigation;
        _identityService = identityService;

    }
    public async Task GenerateJwt(ApplicationUser applicationUser)
    {
        var token = await _identityService.GenerateJwtAsync(applicationUser);
        await _localStorage.SetAsync(TokenKey, token);
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
