using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using CleanArchitecture.Blazor.Domain.Enums;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Constants.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CleanArchitecture.Blazor.Infrastructure.Common.Extensions;
using CleanArchitecture.Blazor.Infrastructure.Extensions;

namespace CleanArchitecture.Blazor.Server.EndPoints;

public class AuthController : Controller
{
    private readonly IDataProtectionProvider _dataProtectionProvider;
    private readonly ILogger<AuthController> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly CustomUserManager _userManager;
    private readonly CustomRoleManager _roleManager;

    public AuthController(
        ILogger<AuthController> logger,
        IDataProtectionProvider dataProtectionProvider,
        CustomUserManager userManager,
        SignInManager<ApplicationUser> signInManager, CustomRoleManager roleManager
    )
    {
        _roleManager = roleManager;
        _logger = logger;
        _dataProtectionProvider = dataProtectionProvider;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet("/auth/login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string token, string returnUrl)
    {
        //TODO need to change the extraction of roles & tenants
        var dataProtector = _dataProtectionProvider.CreateProtector("Login");
        var data = dataProtector.Unprotect(token);
        var parts = data.Split('|');
        var identityUser = await _userManager.FindByIdAsync(parts[0]);
        if (identityUser == null) return Unauthorized();
        var isTokenValid =
            await _userManager.VerifyUserTokenAsync(identityUser, TokenOptions.DefaultProvider, "Login", parts[1]);
        if (isTokenValid)
        {
            var isPersistent = true;
            await _userManager.ResetAccessFailedCountAsync(identityUser);
            await _signInManager.SignInAsync(identityUser, isPersistent);
            identityUser.IsLive = true;
            var resLive= _userManager.UpdateIsLive(identityUser.Id, true);
            //await _userManager.UpdateAsync(identityUser);
            _logger.LogInformation("{@UserName} has successfully logged in", identityUser.UserName);
            return Redirect($"/{returnUrl}");
        }

        return Unauthorized();
    }

    [HttpGet("/auth/externallogin")]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLogin(string provider, string userName, string name, string accessToken)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user is null)
        {
            var admin = await _userManager.FindByNameAsync("administrator") ?? throw new NotFoundException($"Application user administrator Not Found.");
            if (admin.TenantId == null) return null;
            var rl = await _roleManager.FindByNameAsync(RoleNamesEnum.PATIENT.ToString());
            if (rl == null) return null;
            var rolesToInsert = new List<ApplicationUserRoleTenant>() { new ApplicationUserRoleTenant() { TenantId = admin.TenantId, Role = rl } };
            user = new ApplicationUser
            {
                EmailConfirmed = true,
                IsActive = true,
                IsLive = true,
                UserName = userName,
                Email = userName.Any(x => x == '@') ? userName : $"{userName}@{provider}.com",
                Provider = provider,
                DisplayName = name,
                SuperiorId = admin.Id,
                TenantId = admin.TenantId,
                TenantName = admin.TenantName,
                UserRoleTenants = rolesToInsert
            };
            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                return Unauthorized();
            }
            await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, userName, accessToken));
        }

        if (!user.IsActive) return Unauthorized();
        var isPersistent = true;
        await _signInManager.SignInAsync(user, isPersistent);
        return Redirect("/");
    }

    [HttpGet("/auth/logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = _signInManager.Context.User.GetUserId();
        if (!userId.IsNullOrEmptyAndTrimSelf())
        {
            var resultLive =  _userManager.UpdateIsLive(userId, false);
            _logger.LogInformation($"User logout successful({(resultLive>0 ? "Successful" : "Failed")})");
        }
        //var identityUser = await _userManager.FindByIdAsync(userId!) ?? throw new NotFoundException($"Application user not found.");
        //identityUser.IsLive = false;
        //await _userManager.UpdateAsync(identityUser);

        await _signInManager.SignOutAsync();
        return Redirect("/");
    }
}