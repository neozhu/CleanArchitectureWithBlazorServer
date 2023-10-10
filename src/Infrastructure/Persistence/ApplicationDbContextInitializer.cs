using System.Reflection;
using CleanArchitecture.Blazor.Application.Constants;
using CleanArchitecture.Blazor.Application.Constants.ClaimTypes;
using CleanArchitecture.Blazor.Application.Constants.Permission;

using CleanArchitecture.Blazor.Application.Constants.User;
using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence;
public class ApplicationDbContextInitializer
{
    private List<RoleNamesEnum> InternalRoles = CustomUserManager.AllRoleNameEnums
           .Where(e => e > RoleNamesEnum.Hospital)
           .ToList();

    private List<RoleNamesEnum> HospitalRoles = CustomUserManager.AllRoleNameEnums
            .Where(e => e <= RoleNamesEnum.Hospital && e > RoleNamesEnum.DiagnosticCenter)
            .ToList();

    private List<RoleNamesEnum> DiagnosticCenterRoles = CustomUserManager.AllRoleNameEnums
            .Where(e => e <= RoleNamesEnum.DiagnosticCenter && e > RoleNamesEnum.Pharmacy)
            .ToList();

    private List<RoleNamesEnum> PharmacyRoles = CustomUserManager.AllRoleNameEnums
            .Where(e => e <= RoleNamesEnum.Pharmacy && e > RoleNamesEnum.Patient)
            .ToList();
    private readonly ILogger<ApplicationDbContextInitializer> _logger;
    private readonly ApplicationDbContext _context;
    private readonly CustomUserManager _userManager;
    private readonly CustomRoleManager _roleManager;

    public ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger, ApplicationDbContext context, CustomUserManager userManager, CustomRoleManager roleManager)
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
            {
                await _context.Database.MigrateAsync();
            }
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
            TenantBase.GetDefaultTenantStructure().ForEach(t =>
            {
                var tenant = _context.Tenants.Add(TenantStructure.Tenant(t));
                t.Id = tenant.Entity.Id;
            });

            //any more further basic
            await _context.SaveChangesAsync();
        }

        // Default roles //todo change these roles permissions
        var permissions = GetAllPermissions();
        foreach (var roles in TenantBase.GetDefaultTenantStructure().Select(t => t.Roles))
        {
            foreach (var role in roles)
            {
                //var ps = permissions.Where(r => r.StartsWith($"Permissions.{r}"));
                var ps = Perms.PermissionsAll.Where(x => x.roleOrType.Equals(role.Name, StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault().permissions;
                await AddRoleAndPermissions(role, ps);
            }

        }

        // Default users
        var defaultGoogleUsers = new List<(string email, RoleNamesEnum role, TenantTypeEnum tenantType)>()
       {("madhusudhan.veerabhadrappa@gmail.com",InternalRoles.FirstOrDefault() ,TenantTypeEnum.Internal)
       ,("vmadhu203@gmail.com", HospitalRoles.FirstOrDefault(), TenantTypeEnum.HospitalAndStaff)
       ,("vmadhu2023@gmail.com", RoleNamesEnum.Patient, TenantTypeEnum.Patient)
       };

        foreach (var (email, role, tenantType) in defaultGoogleUsers)
        {//need to verify
            if (!_userManager.Users.Any(u => u.Email == email))
            {
                var tenant1 = _context.Tenants.First(x => x.Type == (byte)tenantType);
                var newUser = new ApplicationUser
                {
                    UserName = email,
                    Provider = "Google",
                    IsActive = true,
                    //TenantId = _context.UserRoleTenants.First().Id,//todo need to make change
                    //todo need to change based on selection like whether Patient/Internal/any hospital
                    TenantId = tenant1.Id,//todo need to make change
                    TenantName = tenant1.Name,
                    DisplayName = email,
                    Email = email,
                    EmailConfirmed = true
                    //    , ProfilePictureDataUrl = "https://s.gravatar.com/avatar/78be68221020124c23c665ac54e07074?s=80" 
                };
                await _userManager.CreateAsync(newUser, roles: new List<string>() { role.ToString() });//todo pass roles and tenantids  //, UserName.DefaultPassword);

            }
        }
        // Default data
        // Seed, if necessary
        if (!_context.KeyValues.Any())
        {
            _context.KeyValues.Add(new KeyValue { Name = Picklist.Status, Value = "initialization", Text = "initialization", Description = "Status of workflow" });
            _context.KeyValues.Add(new KeyValue { Name = Picklist.Status, Value = "processing", Text = "processing", Description = "Status of workflow" });
            _context.KeyValues.Add(new KeyValue { Name = Picklist.Status, Value = "pending", Text = "pending", Description = "Status of workflow" });
            _context.KeyValues.Add(new KeyValue { Name = Picklist.Status, Value = "finished", Text = "finished", Description = "Status of workflow" });
            _context.KeyValues.Add(new KeyValue { Name = Picklist.Brand, Value = "Apple", Text = "Apple", Description = "Brand of production" });
            _context.KeyValues.Add(new KeyValue { Name = Picklist.Brand, Value = "MI", Text = "MI", Description = "Brand of production" });
            _context.KeyValues.Add(new KeyValue { Name = Picklist.Brand, Value = "Logitech", Text = "Logitech", Description = "Brand of production" });
            _context.KeyValues.Add(new KeyValue { Name = Picklist.Brand, Value = "Linksys", Text = "Linksys", Description = "Brand of production" });

            _context.KeyValues.Add(new KeyValue { Name = Picklist.Unit, Value = "EA", Text = "EA", Description = "Unit of product" });
            _context.KeyValues.Add(new KeyValue { Name = Picklist.Unit, Value = "KM", Text = "KM", Description = "Unit of product" });
            _context.KeyValues.Add(new KeyValue { Name = Picklist.Unit, Value = "PC", Text = "PC", Description = "Unit of product" });
            _context.KeyValues.Add(new KeyValue { Name = Picklist.Unit, Value = "KG", Text = "KG", Description = "Unit of product" });
            _context.KeyValues.Add(new KeyValue { Name = Picklist.Unit, Value = "ST", Text = "ST", Description = "Unit of product" });
            await _context.SaveChangesAsync();

        }
        if (!_context.Products.Any())
        {
            _context.Products.Add(new Product { Brand = "Apple", Name = "IPhone 13 Pro", Description = "Apple iPhone 13 Pro smartphone. Announced Sep 2021. Features 6.1″ display, Apple A15 Bionic chipset, 3095 mAh battery, 1024 GB storage.", Unit = "EA", Price = 999.98m });
            _context.Products.Add(new Product { Brand = "MI", Name = "MI 12 Pro", Description = "Xiaomi 12 Pro Android smartphone. Announced Dec 2021. Features 6.73″ display, Snapdragon 8 Gen 1 chipset, 4600 mAh battery, 256 GB storage.", Unit = "EA", Price = 199.00m });
            _context.Products.Add(new Product { Brand = "Logitech", Name = "MX KEYS Mini", Description = "Logitech MX Keys Mini Introducing MX Keys Mini – a smaller, smarter, and mightier keyboard made for creators. Type with confidence on a keyboard crafted for efficiency, stability, and...", Unit = "PA", Price = 99.90m });
            await _context.SaveChangesAsync();
        }
    }
    private async Task AddRoleAndPermissions(ApplicationRole role, IEnumerable<string> permissions)
    {
        if (!_roleManager.Roles.Any(r => r.Name == role.Name))
        {
            await _roleManager.CreateAsync(role);
            //todo AspNetRoles extend createasync to add Level parameter also
            if (permissions != null && permissions.Any())
                foreach (var permission in permissions)
                {
                    await _roleManager.AddClaimAsync(role, new Claim(ApplicationClaimTypes.Permission, permission));
                }
        }
    }
}

/*
         _context.UserRoleTenants.Add(new Tenant { Name = "Nanjappa Hospital", Description = "Nanjappa Hospital" });
         _context.UserRoleTenants.Add(new Tenant { Name = "Sarji Hospital", Description = "Sarji Hospital" });
         _context.UserRoleTenants.Add(new Tenant { Name = "Narayana Hospital", Description = "Narayana Hospital" });
         */
/*
 await AddRoleAndPermissions(new ApplicationRole(RoleName.RootAdmin), permissions);
 await AddRoleAndPermissions(new ApplicationRole(RoleName.ElevateAdminGroup), permissions);
 await AddRoleAndPermissions(new ApplicationRole(RoleName.ElevateAdminViewer), permissions);

 await AddRoleAndPermissions(new ApplicationRole(RoleName.Hospital), permissions);
 await AddRoleAndPermissions(new ApplicationRole(RoleName.HospitalAdmin), permissions);
 await AddRoleAndPermissions(new ApplicationRole(RoleName.DoctorHOD), permissions);
 await AddRoleAndPermissions(new ApplicationRole(RoleName.Doctor), permissions);
 await AddRoleAndPermissions(new ApplicationRole(RoleName.DoctorAssistant), permissions);
 await AddRoleAndPermissions(new ApplicationRole(RoleName.Nurse), permissions);
 await AddRoleAndPermissions(new ApplicationRole(RoleName.ViewerHospital), permissions);

 await AddRoleAndPermissions(new ApplicationRole(RoleName.DiagnosticCenter), permissions);
 await AddRoleAndPermissions(new ApplicationRole(RoleName.Diagnostic), permissions);

 await AddRoleAndPermissions(new ApplicationRole(RoleName.Pharmacy), permissions);
 await AddRoleAndPermissions(new ApplicationRole(RoleName.Pharmacist), permissions);

 await AddRoleAndPermissions(new ApplicationRole(RoleName.Patient), permissions);
 */
//  await AddRoleAndPermissions(new ApplicationRole(RoleName.Guest), permissions);

/*      var userRole = new ApplicationRole(RoleName.Basic) { Description = "Basic Group" };
      if (_roleManager.Roles.All(r => r.Name != userRole.Name))
      {
          await _roleManager.CreateAsync(userRole);
          foreach (var permission in permissions)
          {
              if (permission.StartsWith("Permissions.Products"))
                  await _roleManager.AddClaimAsync(userRole, new Claim(ApplicationClaimTypes.Permission, permission));
          }
      } */

/*var administrator = new ApplicationUser { UserName = UserName.Administrator, Provider = "Local", IsActive = true, TenantId = _context.UserRoleTenants.First().Id, TenantName = _context.UserRoleTenants.First().Name, DisplayName = UserName.Administrator, Email = "new163@163.com", EmailConfirmed = true, ProfilePictureDataUrl = "https://s.gravatar.com/avatar/78be68221020124c23c665ac54e07074?s=80" };
var demo = new ApplicationUser { UserName = UserName.Demo, IsActive = true, Provider = "Local", TenantId = _context.UserRoleTenants.First().Id, TenantName = _context.UserRoleTenants.First().Name, DisplayName = UserName.Demo, Email = "neozhu@126.com", EmailConfirmed = true, ProfilePictureDataUrl = "https://s.gravatar.com/avatar/ea753b0b0f357a41491408307ade445e?s=80" };
if (_userManager.Users.All(u => u.UserName != administrator.UserName))
{
    await _userManager.CreateAsync(administrator, UserName.DefaultPassword);
    await _userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name! });
}
if (_userManager.Users.All(u => u.UserName != demo.UserName))
{
    await _userManager.CreateAsync(demo, UserName.DefaultPassword);
    await _userManager.AddToRolesAsync(demo, new[] { userRole.Name! });
}
*/
//await _context.UserTenant.AddAsync(new UserTenant() { UserId = newUser.Id, TenantId = _context.UserRoleTenants.First().Id });
//await _userManager.AddToRolesAsync(newUser, new[] { role! });