using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using CleanArchitecture.Blazor.Application.Common.Security;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Authentication;

public class IdentityAuthenticationService : AuthenticationStateProvider, IAuthenticationService
{

    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private const string KEY = "Identity";
    private const string USERID = "UserId";

    public IdentityAuthenticationService(
        ProtectedLocalStorage protectedLocalStorage,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager
 
  
        )
    {
        _protectedLocalStorage = protectedLocalStorage;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var principal = new ClaimsPrincipal();
        try
        {
            var storedPrincipal = await _protectedLocalStorage.GetAsync<string>(KEY);
            if (storedPrincipal.Success && storedPrincipal.Value is not null)
            {
                var token = storedPrincipal.Value;
                var parts = token.Split('|');
                var identityUser = await _userManager.FindByEmailAsync(parts[0]);
                var isTokenValid = await _userManager.VerifyUserTokenAsync(identityUser, TokenOptions.DefaultProvider, "SignIn", parts[1]);
                if (isTokenValid)
                {
                    var identity = await createIdentityFromApplicationUser(identityUser);
                    principal = new(identity);
                }
            }
        }
        catch
        {

        }
        return new AuthenticationState(principal);
    }

    private async Task<ClaimsIdentity> createIdentityFromApplicationUser(ApplicationUser user)
    {

        var result = new ClaimsIdentity(KEY);
        result.AddClaim(new(ClaimTypes.NameIdentifier, user.Id));
        if (!string.IsNullOrEmpty(user.UserName))
        {
            result.AddClaims(new[] {
                new Claim(ClaimTypes.Name, user.UserName)
            });
        }
        if (!string.IsNullOrEmpty(user.Site))
        {
            result.AddClaims(new[] {
                new Claim(ClaimTypes.Locality, user.Site)
            });
        }
        if (!string.IsNullOrEmpty(user.Email))
        {
            result.AddClaims(new[] {
                new Claim(ClaimTypes.Email, user.Email)
            });
        }
        if (!string.IsNullOrEmpty(user.ProfilePictureDataUrl))
        {
            result.AddClaims(new[] {
                new Claim(ApplicationClaimTypes.ProfilePictureDataUrl, user.ProfilePictureDataUrl)
            });
        }
        if (!string.IsNullOrEmpty(user.DisplayName))
        {
            result.AddClaims(new[] {
                new Claim(ClaimTypes.GivenName, user.DisplayName)
            });
        }
        var appuser = await _userManager.FindByIdAsync(user.Id);
        var roles = await _userManager.GetRolesAsync(appuser);
        foreach (var rolename in roles)
        {
            var role = await _roleManager.FindByNameAsync(rolename);
            var claims = await _roleManager.GetClaimsAsync(role);
            foreach (var claim in claims)
            {
                result.AddClaim(claim);
            }
            result.AddClaims(new[] {
                new Claim(ClaimTypes.Role, rolename) });

        }
        return result;
    }


    public async Task<bool> Login(LoginFormModel request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);
        var valid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (valid)
        {
            var token = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, "SignIn");
            var data = $"{user.Email}|{token}";
            await _protectedLocalStorage.SetAsync(KEY, data);
            await _protectedLocalStorage.SetAsync(USERID, user.Id);
            var identity = await createIdentityFromApplicationUser(user);
            var principal = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        }
        return valid;
    }

    public async Task Logout()
    {
        await _protectedLocalStorage.DeleteAsync(KEY);
        await _protectedLocalStorage.DeleteAsync(USERID);
        var principal = new ClaimsPrincipal();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }
}
