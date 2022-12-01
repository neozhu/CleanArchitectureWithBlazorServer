
using CleanArchitecture.Blazor.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blazor.Server.UI.EndPoints;
public class AuthController : Controller
{
    private readonly IDataProtectionProvider _dataProtectionProvider;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthController(
        IDataProtectionProvider dataProtectionProvider,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager
    )
    {
        _dataProtectionProvider = dataProtectionProvider;
        _userManager = userManager;
        _signInManager = signInManager;
    }
    [HttpGet("/auth/login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string token)
    {
        var dataProtector = _dataProtectionProvider.CreateProtector("Login");
        var data = dataProtector.Unprotect(token);
        var parts = data.Split('|');
        var identityUser = await _userManager.FindByIdAsync(parts[0]);

        if (identityUser == null)
        {
            return Unauthorized();
        }

        var isTokenValid = await _userManager.VerifyUserTokenAsync(identityUser, TokenOptions.DefaultProvider,"Login", parts[1]);

        if (isTokenValid)
        {
            var isPersistent = true;

            await _userManager.ResetAccessFailedCountAsync(identityUser);

            await _signInManager.SignInAsync(identityUser, isPersistent);

            return Redirect("/");
        }

        return Unauthorized();
    }
}