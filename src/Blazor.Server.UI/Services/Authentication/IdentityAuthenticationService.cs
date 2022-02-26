using System.Security.Claims;
using Blazor.Server.UI.Models.Authentication;
using CleanArchitecture.Blazor.Infrastructure.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using CleanArchitecture.Blazor.Infrastructure.Constants.ClaimTypes;


namespace Blazor.Server.UI.Services.Authentication;

public class IdentityAuthenticationService : AuthenticationStateProvider, IAuthenticationService
{
    private readonly IDataProtector _dataProtector;
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private const string KEY = "Identity";

    public IdentityAuthenticationService(
        IDataProtectionProvider dataProtectionProvider,
        ProtectedLocalStorage protectedLocalStorage,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager
 
  
        )
    {
        _dataProtector = dataProtectionProvider.CreateProtector("SignIn");
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
                var data = _dataProtector.Unprotect(token);
                var parts = data.Split('|');
                var identityUser = await _userManager.FindByIdAsync(parts[0]);
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


    public async Task Login(LoginFormModel request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);
        var valid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (valid)
        {
            var token = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, "SignIn");
            var data = $"{user.Id}|{token}";
            var pdata = _dataProtector.Protect(data);
            await _protectedLocalStorage.SetAsync(KEY, pdata);
            var identity = await createIdentityFromApplicationUser(user);
            var principal = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        }
    }

    public async Task Logout()
    {
        await _protectedLocalStorage.DeleteAsync(KEY);
        var principal = new ClaimsPrincipal();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }
}
