// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Infrastructure.Constants.Role;
using System.Reflection;


namespace CleanArchitecture.Blazor.Infrastructure.Persistence;

public static class ApplicationDbContextSeed
{
    public static async Task SeedDefaultUserAsync(ApplicationDbContext context,UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        
        var administratorRole = new ApplicationRole(RoleConstants.AdministratorRole) { Description = "Admin Group" };
        var userRole = new ApplicationRole(RoleConstants.BasicRole) { Description = "Basic Group" };

        if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await roleManager.CreateAsync(administratorRole);
            await roleManager.CreateAsync(userRole);
            var Permissions = GetAllPermissions();
            foreach (var permission in Permissions)
            {
                await roleManager.AddClaimAsync(administratorRole, new System.Security.Claims.Claim(ApplicationClaimTypes.Permission, permission));
                if(permission.StartsWith("Permissions.Products"))
                  await roleManager.AddClaimAsync(userRole, new System.Security.Claims.Claim(ApplicationClaimTypes.Permission, permission));
            }
        }

        var administrator = new ApplicationUser {  UserName = "administrator", Provider="Local", IsActive = true, TenantId = context.Tenants.First().Id, TenantName= context.Tenants.First().Name, DisplayName = "Administrator", Email = "new163@163.com", EmailConfirmed = true, ProfilePictureDataUrl = $"https://s.gravatar.com/avatar/78be68221020124c23c665ac54e07074?s=80" };
        var demo = new ApplicationUser { UserName = "Demo", IsActive = true, Provider = "Local", TenantId = context.Tenants.First().Id, TenantName = context.Tenants.First().Name, DisplayName = "Demo", Email = "neozhu@126.com", EmailConfirmed = true, ProfilePictureDataUrl = $"https://s.gravatar.com/avatar/ea753b0b0f357a41491408307ade445e?s=80" };

        if (userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await userManager.CreateAsync(administrator, RoleConstants.DefaultPassword);
            await userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
            await userManager.CreateAsync(demo, RoleConstants.DefaultPassword);
            await userManager.AddToRolesAsync(demo, new[] { userRole.Name });
        }

    }
    private static IEnumerable<string> GetAllPermissions()
    {
        var allPermissions = new List<string>();
        var modules = typeof(Permissions).GetNestedTypes();

        foreach (var module in modules)
        {
            var moduleName = string.Empty;
            var moduleDescription = string.Empty;

            var fields = module.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            foreach (var fi in fields)
            {
                var propertyValue = fi.GetValue(null);

                if (propertyValue is not null)
                    allPermissions.Add(propertyValue.ToString());
            }
        }

        return allPermissions;
    }

    public static async Task SeedSampleDataAsync(ApplicationDbContext context)
    {
        //Seed, if necessary
        if (!context.Tenants.Any())
        {
            context.Tenants.Add(new Domain.Entities.Tenant() { Name = "Master", Description = "Master Site" });
            context.Tenants.Add(new Domain.Entities.Tenant() { Name = "Slave", Description = "Slave Site" });
            await context.SaveChangesAsync();

        }
        if (!context.KeyValues.Any())
        {
            context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Status", Value = "initialization", Text = "initialization", Description = "Status of workflow" });
            context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Status", Value = "processing", Text = "processing", Description = "Status of workflow" });
            context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Status", Value = "pending", Text = "pending", Description = "Status of workflow" });
            context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Status", Value = "finished", Text = "finished", Description = "Status of workflow" });
            context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Brand", Value = "Apple", Text = "Apple", Description = "Brand of production" });
            context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Brand", Value = "MI", Text = "MI", Description = "Brand of production" });
            context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Brand", Value = "Logitech", Text = "Logitech", Description = "Brand of production" });
            context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Brand", Value = "Linksys", Text = "Linksys", Description = "Brand of production" });

            context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Unit", Value = "EA", Text = "EA", Description = "Unit of product" });
            context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Unit", Value = "KM", Text = "KM", Description = "Unit of product" });
            context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Unit", Value = "PC", Text = "PC", Description = "Unit of product" });
            context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Unit", Value = "KG", Text = "KG", Description = "Unit of product" });
            context.KeyValues.Add(new Domain.Entities.KeyValue() { Name = "Unit", Value = "ST", Text = "ST", Description = "Unit of product" });
            await context.SaveChangesAsync();
          
        }
        if (!context.Products.Any())
        {
            context.Products.Add(new Domain.Entities.Product() { Brand= "Apple", Name = "IPhone 13 Pro", Description= "Apple iPhone 13 Pro smartphone. Announced Sep 2021. Features 6.1″ display, Apple A15 Bionic chipset, 3095 mAh battery, 1024 GB storage.", Unit="EA",Price=999.98m });
            context.Products.Add(new Domain.Entities.Product() { Brand = "MI", Name = "MI 12 Pro", Description = "Xiaomi 12 Pro Android smartphone. Announced Dec 2021. Features 6.73″ display, Snapdragon 8 Gen 1 chipset, 4600 mAh battery, 256 GB storage.", Unit = "EA", Price = 199.00m });
            context.Products.Add(new Domain.Entities.Product() { Brand = "Logitech",  Name = "MX KEYS Mini", Description = "Logitech MX Keys Mini Introducing MX Keys Mini – a smaller, smarter, and mightier keyboard made for creators. Type with confidence on a keyboard crafted for efficiency, stability, and...", Unit = "PA", Price = 99.90m });
            await context.SaveChangesAsync();
        }
    }
}
