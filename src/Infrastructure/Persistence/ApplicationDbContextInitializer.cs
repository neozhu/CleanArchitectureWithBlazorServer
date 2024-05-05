using System.Reflection;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Constants.ClaimTypes;
using CleanArchitecture.Blazor.Infrastructure.Constants.Role;
using CleanArchitecture.Blazor.Infrastructure.Constants.User;
using CleanArchitecture.Blazor.Infrastructure.PermissionSet;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence;

public class ApplicationDbContextInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApplicationDbContextInitializer> _logger;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger,
        ApplicationDbContext context, UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer() || _context.Database.IsNpgsql() || _context.Database.IsSqlite())
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
            await TrySeedAsync();
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

    private async Task TrySeedAsync()
    {
        // Default tenants
        if (!_context.Tenants.Any())
        {
            _context.Tenants.Add(new Tenant { Name = "Master", Description = "Master Site" });
            _context.Tenants.Add(new Tenant { Name = "Slave", Description = "Slave Site" });
            await _context.SaveChangesAsync();
        }

        // Default roles
        var administratorRole = new ApplicationRole(RoleName.Admin) { Description = "Admin Group", TenantId= _context.Tenants.First().Id };
        var userRole = new ApplicationRole(RoleName.Basic) { Description = "Basic Group", TenantId = _context.Tenants.First().Id };
        var permissions = GetAllPermissions();
        if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);

            foreach (var permission in permissions)
                await _roleManager.AddClaimAsync(administratorRole,
                    new Claim(ApplicationClaimTypes.Permission, permission));
        }

        if (_roleManager.Roles.All(r => r.Name != userRole.Name))
        {
            await _roleManager.CreateAsync(userRole);
            foreach (var permission in permissions)
                if (permission.StartsWith("Permissions.Products"))
                    await _roleManager.AddClaimAsync(userRole, new Claim(ApplicationClaimTypes.Permission, permission));
        }

        // Default users
        var administrator = new ApplicationUser
        {
            UserName = UserName.Administrator,
            Provider = "Local",
            IsActive = true,
            TenantId = _context.Tenants.First().Id,
            DisplayName = UserName.Administrator, Email = "new163@163.com", EmailConfirmed = true,
            ProfilePictureDataUrl = "https://s.gravatar.com/avatar/78be68221020124c23c665ac54e07074?s=80",
            TwoFactorEnabled = false
        };
        var demo = new ApplicationUser
        {
            UserName = UserName.Demo,
            IsActive = true,
            Provider = "Local",
            TenantId = _context.Tenants.First().Id,
            DisplayName = UserName.Demo, Email = "neozhu@126.com",
            EmailConfirmed = true,
            ProfilePictureDataUrl = "https://s.gravatar.com/avatar/ea753b0b0f357a41491408307ade445e?s=80"
        };


        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, UserName.DefaultPassword);
            await _userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name! });
            //await _userManager.SetTwoFactorEnabledAsync(administrator, true);
        }

        if (_userManager.Users.All(u => u.UserName != demo.UserName))
        {
            await _userManager.CreateAsync(demo, UserName.DefaultPassword);
            await _userManager.AddToRolesAsync(demo, new[] { userRole.Name! });
        }

        // Default data
        // Seed, if necessary
        if (!_context.KeyValues.Any())
        {
            _context.KeyValues.Add(new KeyValue
            {
                Name = Picklist.Status, Value = "initialization", Text = "initialization",
                Description = "Status of workflow"
            });
            _context.KeyValues.Add(new KeyValue
            {
                Name = Picklist.Status, Value = "processing", Text = "processing", Description = "Status of workflow"
            });
            _context.KeyValues.Add(new KeyValue
            { Name = Picklist.Status, Value = "pending", Text = "pending", Description = "Status of workflow" });
            _context.KeyValues.Add(new KeyValue
            { Name = Picklist.Status, Value = "finished", Text = "finished", Description = "Status of workflow" });
            _context.KeyValues.Add(new KeyValue
            { Name = Picklist.Brand, Value = "Apple", Text = "Apple", Description = "Brand of production" });
            _context.KeyValues.Add(new KeyValue
            { Name = Picklist.Brand, Value = "MI", Text = "MI", Description = "Brand of production" });
            _context.KeyValues.Add(new KeyValue
            { Name = Picklist.Brand, Value = "Logitech", Text = "Logitech", Description = "Brand of production" });
            _context.KeyValues.Add(new KeyValue
            { Name = Picklist.Brand, Value = "Linksys", Text = "Linksys", Description = "Brand of production" });

            _context.KeyValues.Add(new KeyValue
            { Name = Picklist.Unit, Value = "EA", Text = "EA", Description = "Unit of product" });
            _context.KeyValues.Add(new KeyValue
            { Name = Picklist.Unit, Value = "KM", Text = "KM", Description = "Unit of product" });
            _context.KeyValues.Add(new KeyValue
            { Name = Picklist.Unit, Value = "PC", Text = "PC", Description = "Unit of product" });
            _context.KeyValues.Add(new KeyValue
            { Name = Picklist.Unit, Value = "KG", Text = "KG", Description = "Unit of product" });
            _context.KeyValues.Add(new KeyValue
            { Name = Picklist.Unit, Value = "ST", Text = "ST", Description = "Unit of product" });
            await _context.SaveChangesAsync();
        }

        if (!_context.Products.Any())
        {
            _context.Products.Add(new Product
            {
                Brand = "Apple", Name = "IPhone 13 Pro",
                Description =
                    "Apple iPhone 13 Pro smartphone. Announced Sep 2021. Features 6.1″ display, Apple A15 Bionic chipset, 3095 mAh battery, 1024 GB storage.",
                Unit = "EA", Price = 999.98m
            });
            _context.Products.Add(new Product
            {
                Brand = "MI", Name = "MI 12 Pro",
                Description =
                    "Xiaomi 12 Pro Android smartphone. Announced Dec 2021. Features 6.73″ display, Snapdragon 8 Gen 1 chipset, 4600 mAh battery, 256 GB storage.",
                Unit = "EA", Price = 199.00m
            });
            _context.Products.Add(new Product
            {
                Brand = "Logitech", Name = "MX KEYS Mini",
                Description =
                    "Logitech MX Keys Mini Introducing MX Keys Mini – a smaller, smarter, and mightier keyboard made for creators. Type with confidence on a keyboard crafted for efficiency, stability, and...",
                Unit = "PA", Price = 99.90m
            });
            await _context.SaveChangesAsync();
        }
    }
}