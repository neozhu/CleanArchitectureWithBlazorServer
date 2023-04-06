using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
public  class BlazorAuthStateProvider : AuthenticationStateProvider
{
    private readonly AccessTokenProvider _tokenProvider;

    public BlazorAuthStateProvider(AccessTokenProvider  tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var claimsPrincipal =await _tokenProvider.GetClaimsPrincipal();
        return new AuthenticationState(claimsPrincipal);
    }
}

