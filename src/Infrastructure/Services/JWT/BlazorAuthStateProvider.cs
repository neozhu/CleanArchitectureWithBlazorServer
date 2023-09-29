namespace CleanArchitecture.Blazor.Infrastructure.Services.JWT;
public  class BlazorAuthStateProvider : AuthenticationStateProvider
{
    private readonly IAccessTokenProvider _tokenProvider;

    public BlazorAuthStateProvider(IAccessTokenProvider  tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var claimsPrincipal =await _tokenProvider.GetClaimsPrincipal();
        return new AuthenticationState(claimsPrincipal);
    }
}

