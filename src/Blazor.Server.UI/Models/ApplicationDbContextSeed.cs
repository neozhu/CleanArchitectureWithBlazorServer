using Microsoft.AspNetCore.Identity;

namespace Blazor.Server.UI.Models;
public static class ApplicationDbContextSeed
{
    public static async Task SeedDefaultUserAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        var administratorRole = new IdentityRole() { Name = "admin" };
        var userRole = new IdentityRole() { Name="basic"};

        if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await roleManager.CreateAsync(administratorRole);
            await roleManager.CreateAsync(userRole);
        }

        var administrator = new IdentityUser { UserName = "root", Email= "root@mudblazor.com" };

        if (userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await userManager.CreateAsync(administrator, "Root@123!");
            await userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
        }

    }
 
}
