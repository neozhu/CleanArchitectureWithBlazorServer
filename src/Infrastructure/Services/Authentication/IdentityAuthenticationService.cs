using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using CleanArchitecture.Blazor.Application.Common.Security;
using System.Text;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Authentication;

public class IdentityAuthenticationService : AuthenticationStateProvider, IAuthenticationService
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private const string KEY = "Basic";
    private const string USERID = "UserId";
    private const string USERNAME = "UserName";
    private const string CLAIMSIDENTITY = "ClaimsIdentity";
    public IdentityAuthenticationService(
        ProtectedLocalStorage protectedLocalStorage,
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager
        )
    {
        _protectedLocalStorage = protectedLocalStorage;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity());
        try
        {
            var storedClaimsIdentity = await _protectedLocalStorage.GetAsync<string>(CLAIMSIDENTITY);
            if (storedClaimsIdentity.Success && storedClaimsIdentity.Value is not null)
            {
                var buffer = Convert.FromBase64String(storedClaimsIdentity.Value);
                using (var deserializationStream = new MemoryStream(buffer))
                {
                    var identity = new ClaimsIdentity(new BinaryReader(deserializationStream, Encoding.UTF8));
                    principal = new(identity);
                }
            }
        }
        catch (Exception e)
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
        await _semaphore.WaitAsync();
        try
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            var valid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (valid)
            {

                var identity = await createIdentityFromApplicationUser(user);
                using (var memoryStream = new MemoryStream())
                using (var binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, true))
                {
                    identity.WriteTo(binaryWriter);
                    var base64 = Convert.ToBase64String(memoryStream.ToArray());
                    await _protectedLocalStorage.SetAsync(CLAIMSIDENTITY, base64);
                }
                await _protectedLocalStorage.SetAsync(USERID, user.Id);
                await _protectedLocalStorage.SetAsync(USERNAME, user.UserName);
                var principal = new ClaimsPrincipal(identity);
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
            }
            return valid;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task Logout()
    {
        await _protectedLocalStorage.DeleteAsync(CLAIMSIDENTITY);
        await _protectedLocalStorage.DeleteAsync(USERID);
        await _protectedLocalStorage.DeleteAsync(USERNAME);
        var principal = new ClaimsPrincipal();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }
}
