﻿using System;
using System.Reflection;
using CleanArchitecture.Blazor.Application.Common.Constants;
using CleanArchitecture.Blazor.Application.Common.Security;
using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence;

public class ApplicationDbContextInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApplicationDbContextInitializer> _logger;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger,
        IDbContextFactory<ApplicationDbContext> dbContextFactory,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _logger = logger;
        _context = dbContextFactory.CreateDbContext();
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsRelational())
                await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await SeedTenantsAsync();
            await SeedRolesAsync();
            await SeedUsersAsync();
            await SeedDataAsync();
            _context.ChangeTracker.Clear();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
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
                    allPermissions.Add((string)propertyValue);
            }
        }

        return allPermissions;
    }




    private async Task SeedTenantsAsync()
    {
        if (await _context.Tenants.AnyAsync()) return;

        _logger.LogInformation("Seeding organizations...");
        var tenants = new[]
        {
                new Tenant { Name = "Main", Description = "Main Site" },
                new Tenant { Name = "Europe", Description = "Europe Site" }
            };

        await _context.Tenants.AddRangeAsync(tenants);
        await _context.SaveChangesAsync();
    }

    private async Task SeedRolesAsync()
    {
        var adminRoleName = Roles.Admin;
        var userRoleName = Roles.Basic;

        if (await _roleManager.RoleExistsAsync(adminRoleName)) return;

        _logger.LogInformation("Seeding roles...");
        var administratorRole = new ApplicationRole(adminRoleName)
        {
            Description = "Admin Group",
            CreatedAt= DateTime.UtcNow,
        };
        var userRole = new ApplicationRole(userRoleName)
        {
            Description = "Basic Group",
            CreatedAt = DateTime.UtcNow,
        };

        await _roleManager.CreateAsync(administratorRole);
        await _roleManager.CreateAsync(userRole);

        var permissions = GetAllPermissions();

        foreach (var permission in permissions)
        {
            var claim = new Claim(ApplicationClaimTypes.Permission, permission);
            await _roleManager.AddClaimAsync(administratorRole, claim);

            if (permission.StartsWith("Permissions.Products"))
            {
                await _roleManager.AddClaimAsync(userRole, claim);
            }
        }
    }

    private async Task SeedUsersAsync()
    {
        if (await _userManager.Users.AnyAsync()) return;

        _logger.LogInformation("Seeding users...");
        var tenants = await _context.Tenants.ToListAsync();
        var adminUser = new ApplicationUser
        {
            UserName = Users.Administrator,
            Provider = "Local",
            IsActive = true,
            TenantId = (await _context.Tenants.FirstAsync()).Id,
            DisplayName = Users.Administrator,
            Email = "admin@example.com",
            EmailConfirmed = true,
            ProfilePictureDataUrl = "https://s.gravatar.com/avatar/78be68221020124c23c665ac54e07074?s=80",
            LanguageCode="en-US",
            TimeZoneId= "Asia/Shanghai",
            TwoFactorEnabled = false,
            CreatedAt=DateTime.UtcNow,
            TenantUsers = tenants.Select(t => new TenantUser { TenantId = t.Id }).ToList()
        };
        await _userManager.CreateAsync(adminUser, Users.DefaultPassword);
        await _userManager.AddToRoleAsync(adminUser, Roles.Admin);
        var demoUser = new ApplicationUser
        {
            UserName = Users.Demo,
            IsActive = true,
            Provider = "Local",
            TenantId = (await _context.Tenants.FirstAsync()).Id,
            DisplayName = Users.Demo,
            SuperiorId = adminUser.Id,
            Email = "demo@example.com",
            EmailConfirmed = true,
            LanguageCode = "de-DE",
            TimeZoneId = "Europe/Berlin",
            TenantUsers = new List<TenantUser> { new TenantUser { TenantId = tenants.First().Id } },
            ProfilePictureDataUrl = "https://s.gravatar.com/avatar/ea753b0b0f357a41491408307ade445e?s=80",
            CreatedAt = DateTime.UtcNow
        };

       

        await _userManager.CreateAsync(demoUser, Users.DefaultPassword);
        await _userManager.AddToRoleAsync(demoUser, Roles.Basic);
    }

    private async Task SeedDataAsync()
    {
        if (!await _context.PicklistSets.AnyAsync())
        {

            _logger.LogInformation("Seeding key values...");
            var keyValues = new[]
            {
                new PicklistSet
                {
                    Name = Picklist.Status,
                    Value = "initialization",
                    Text = "Initialization",
                    Description = "Status of workflow"
                },
                new PicklistSet
                {
                    Name = Picklist.Status,
                    Value = "processing",
                    Text = "Processing",
                    Description = "Status of workflow"
                },
                new PicklistSet
                {
                    Name = Picklist.Status,
                    Value = "pending",
                    Text = "Pending",
                    Description = "Status of workflow"
                },
                new PicklistSet
                {
                    Name = Picklist.Status,
                    Value = "done",
                    Text = "Done",
                    Description = "Status of workflow"
                },
                new PicklistSet
                {
                    Name = Picklist.Brand,
                    Value = "Apple",
                    Text = "Apple",
                    Description = "Brand of production"
                },
                new PicklistSet
                {
                    Name = Picklist.Brand,
                    Value = "Google",
                    Text = "Google",
                    Description = "Brand of production"
                },
                new PicklistSet
                {
                    Name = Picklist.Brand,
                    Value = "Microsoft",
                    Text = "Microsoft",
                    Description = "Brand of production"
                },
                new PicklistSet
                {
                    Name = Picklist.Unit,
                    Value = "EA",
                    Text = "EA",
                    Description = "Unit of product"
                },
                new PicklistSet
                {
                    Name = Picklist.Unit,
                    Value = "KM",
                    Text = "KM",
                    Description = "Unit of product"
                },
                new PicklistSet
                {
                    Name = Picklist.Unit,
                    Value = "PC",
                    Text = "PC",
                    Description = "Unit of product"
                },
                new PicklistSet
                {
                    Name = Picklist.Unit,
                    Value = "L",
                    Text = "L",
                    Description = "Unit of product"
                }
            };

            await _context.PicklistSets.AddRangeAsync(keyValues);
            await _context.SaveChangesAsync();
        }

        if (!await _context.Products.AnyAsync())
        {

            _logger.LogInformation("Seeding products...");
            var products = new[]
            {
                new Product
                {
                    Brand = "Apple",
                    Name = "IPhone 13 Pro",
                    Description =
                    "Apple iPhone 13 Pro smartphone. Announced Sep 2021. Features 6.1″ display, Apple A15 Bionic chipset, 3095 mAh battery, 1024 GB storage.",
                    Unit = "EA",
                    Price = 999.98m
                },
                new Product
                {
                    Brand = "Sony",
                    Name = "WH-1000XM4",
                    Description = "Sony WH-1000XM4 Wireless Noise-Canceling Over-Ear Headphones. Features industry-leading noise cancellation, up to 30 hours of battery life, touch sensor controls.",
                    Unit = "EA",
                    Price = 349.99m
                },
                new Product
                {
                    Brand = "Nintendo",
                    Name = "Switch OLED Model",
                    Description = "Nintendo Switch OLED Model console. Released October 2021. Features 7″ OLED screen, 64GB internal storage, enhanced audio, dock with wired LAN port.",
                    Unit = "EA",
                    Price = 349.99m
                },
                new Product
                {
                    Brand = "Apple",
                    Name = "MacBook Air M1",
                    Description = "Apple MacBook Air with M1 chip. Features 13.3″ Retina display, Apple M1 chip with 8‑core CPU, 8GB RAM, 256GB SSD storage, up to 18 hours of battery life.",
                    Unit = "EA",
                    Price = 999.99m
                }

            };

            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
        }
    }
}
