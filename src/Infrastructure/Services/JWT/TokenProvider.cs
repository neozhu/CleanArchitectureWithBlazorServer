using System.Security.Cryptography;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
public class TokenProvider
{
    private readonly string TokenKey = nameof(TokenKey);
    private readonly ProtectedLocalStorage _localStorage;
    private readonly NavigationManager _navigation;
    private readonly IIdentityService _identityService;

    public TokenProvider(ProtectedLocalStorage localStorage, NavigationManager navigation, IIdentityService identityService)
    {
        _localStorage = localStorage;
        _navigation = navigation;
        _identityService = identityService;

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
            await RemoveAuthDataFromStorageAsync();
        }
        return new ClaimsPrincipal(new ClaimsIdentity());
    }


    private async Task RemoveAuthDataFromStorageAsync()
    {
        await _localStorage.DeleteAsync(TokenKey);
        _navigation.NavigateTo("/", true);
    }
}
