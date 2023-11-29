using System.Security.Claims;
using CleanArchitecture.Blazor.Infrastructure.Services.JWT;
using Microsoft.AspNetCore.Components.Authorization;

namespace CleanArchitecture.Blazor.Server.UI.Services;
public  class BlazorAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IAccessTokenProvider _tokenProvider;

    public BlazorAuthenticationStateProvider(IAccessTokenProvider  tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var claimsPrincipal =await _tokenProvider.GetClaimsPrincipal();
        return new AuthenticationState(claimsPrincipal);
    }
    public void MarkUserAsAuthenticated(ClaimsPrincipal authenticatedUser)
    {
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }
    public void MarkUserAsLoggedOut()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));
        NotifyAuthenticationStateChanged(authState);
    }
}

