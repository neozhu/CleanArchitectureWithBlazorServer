using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using CleanArchitecture.Blazor.Application.Common.Security;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Authentication;

public class IdentityAuthenticationService : AuthenticationStateProvider, IAuthenticationService
{

    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly IServiceProvider _serviceProvider;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private const string KEY = "Identity";
    private const string USERID = "UserId";
    private const string USERNAME = "UserName";
    private const string PURPOSE = "SignIn";
    public IdentityAuthenticationService(
        ProtectedLocalStorage protectedLocalStorage,
        IServiceProvider serviceProvider
        )
    {
        _protectedLocalStorage = protectedLocalStorage;
        _serviceProvider = serviceProvider;
        _userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        _roleManager = _serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
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
                var identityUser = await _userManager.FindByIdAsync(parts[0]);
                var isTokenValid = await _userManager.VerifyUserTokenAsync(identityUser, TokenOptions.DefaultProvider, "SignIn", parts[1]);
                if (isTokenValid)
                {
                    var identity = await createIdentityFromApplicationUser(identityUser);
                    principal = new(identity);
                }
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
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
        if (!string.IsNullOrEmpty(user.PhoneNumber))
        {
            result.AddClaims(new[] {
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber)
            });
        }
        var roles = await _userManager.GetRolesAsync(user);
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
            var token = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, PURPOSE);
            var data = $"{user.Id}|{token}";
            await _protectedLocalStorage.SetAsync(KEY, data);
            await _protectedLocalStorage.SetAsync(USERID, user.Id);
            await _protectedLocalStorage.SetAsync(USERNAME, user.UserName);
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
        await _protectedLocalStorage.DeleteAsync(USERNAME);
        var principal = new ClaimsPrincipal();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }
}
