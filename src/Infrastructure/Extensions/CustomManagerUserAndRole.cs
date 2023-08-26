using CleanArchitecture.Blazor.Application.Constants.Role;
using CleanArchitecture.Blazor.Application.Constants.User;
using CleanArchitecture.Blazor.Domain.Enums;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;

public class CustomUserManager : UserManager<ApplicationUser>
{
    public const string defaultTenantId = "";//todo make it loaded as per db
    private readonly CustomRoleManager _roleManager;
    private readonly IServiceProvider _serviceProvider;
    public CustomUserManager(
         IUserStore<ApplicationUser> store,
         IOptions<IdentityOptions> optionsAccessor,
         IPasswordHasher<ApplicationUser> passwordHasher,
         IEnumerable<IUserValidator<ApplicationUser>> userValidators,
         IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
         ILookupNormalizer keyNormalizer,
         IdentityErrorDescriber errors,
         IServiceProvider services,
         ILogger<UserManager<ApplicationUser>> logger, CustomRoleManager roleManager)
         : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        _roleManager = roleManager;
    }
    /*
    public async override Task<IdentityResult> CreateAsync(ApplicationUser user)
    {
        return await CreateAsync(user, null);
    }
    public async override Task<IdentityResult> CreateAsync(ApplicationUser user, string password = null)
    {
        if (string.IsNullOrEmpty(user.TenantId)) user.TenantId = defaultTenantId;
        if (user.UserRoles == null || !user.UserRoles.Any())
            return await CreateWithDefaultRolesAsync(user, tenantId: user.TenantId);
        var result = string.IsNullOrEmpty(password) ? await base.CreateAsync(user) : await base.CreateAsync(user, password);
        return result;
    }
    public async Task<IdentityResult> CreateAsync(ApplicationUser user, List<string> roles, string tenantId = null, string password = null)
    {
        if (string.IsNullOrEmpty(user.TenantId)) user.TenantId = defaultTenantId;
        return await CreateAsync(user, roles, user.TenantId);
    }

    public async Task<IdentityResult> CreateAsync(ApplicationUser user, List<string> roles, string tenantId)
    {
        if (string.IsNullOrEmpty(tenantId)) tenantId = defaultTenantId;
        if (roles == null || !roles.Any()) return await CreateWithDefaultRolesAsync(user, tenantId);
        user.UserRoles = new List<ApplicationUserRole>();
        roles.ForEach(c =>
        {
            var roleId = (_roleManager.FindByNameAsync(c).Result)?.Id;
            if (roleId != null) user.UserRoles.Add(new ApplicationUserRole() { RoleId = roleId, TenantId = tenantId });
        });

        return await CreateAsync(user, null, roles, tenantId);
    }


    public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password, List<string> roles)
    {
        if (string.IsNullOrEmpty(user.TenantId)) user.TenantId = defaultTenantId;
        if (roles == null || !roles.Any()) return await CreateWithDefaultRolesAsync(user, user.TenantId);
        user.UserRoles = new List<ApplicationUserRole>();
        roles.ForEach(c =>
        {
            var roleId = (_roleManager.FindByNameAsync(c).Result)?.Id;
            if (roleId != null) user.UserRoles.Add(new ApplicationUserRole() { RoleId = roleId, TenantId = tenantId });
        });

        return await CreateAsync(user, password, roles, tenantId);
    }
    */
    public async Task<IdentityResult> CreateWithDefaultRolesAsync(ApplicationUser user, string tenantId = null, string password = null)
    {
        return await CreateAsync(user, new List<string> { RoleName.DefaultRole1 }, tenantId, password);
    }


    public async Task<IdentityResult> CreateAsync(ApplicationUser user, List<string> roles = null, string tenantId = null, string password = null)
    {
        if (string.IsNullOrEmpty(tenantId)) tenantId = defaultTenantId;
        if (string.IsNullOrEmpty(user.TenantId)) user.TenantId = tenantId;//this overrides already assigned tenant,had to make sure
        if (roles == null || !roles.Any()) return await CreateWithDefaultRolesAsync(user, user.TenantId, password);
        user.UserRoles = new List<ApplicationUserRole>();//here it ignores already exisitng UserRoles //TODO need to tink of this
        roles.ForEach(c =>
        {
            var roleId = (_roleManager.FindByNameAsync(c).Result)?.Id;
            if (roleId != null) user.UserRoles.Add(new ApplicationUserRole() { RoleId = roleId, TenantId = user.TenantId });
        });

        var result = string.IsNullOrEmpty(password) ? await base.CreateAsync(user) : await base.CreateAsync(user, password);
        return result;
    }

    public async Task<IdentityResult> UpdateRolesAsyncWithTenantId(ApplicationUser user, List<string> newRoleNames, string tenantId)
    {
        // Retrieve the current roles of the user for the given tenant
        var currentRoles = await GetRoleNamesAsync(userId: user.Id, tenantId: tenantId, roleId: null);
        if (currentRoles != null && currentRoles.Any())
        {
            // Calculate roles to be added and removed
            var rolesToAdd = newRoleNames.Except(currentRoles);
            var rolesToRemove = currentRoles.Except(newRoleNames);

            // Add new roles
            foreach (var roleName in rolesToAdd)
            {
                await AddToRoleAsync(user, roleName);
            }

            // Remove old roles
            foreach (var roleName in rolesToRemove)
            {
                await RemoveFromRoleAsync(user, roleName);
            }
        }

        return IdentityResult.Success;
    }

    public async Task<IList<ApplicationUserRole>> GetUserRoleTenantIdsAsync(string userId, string roleId = null, string tenantId = null)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); // Replace with your DbContext

        var query = dbContext.UserRoles.AsQueryable();
        if (!string.IsNullOrEmpty(tenantId))
            query = query.Where(role => role.TenantId == tenantId);
        if (!string.IsNullOrEmpty(roleId))
            query = query.Where(role => role.RoleId == roleId);
        if (!string.IsNullOrEmpty(userId))
            query = query.Where(role => role.UserId == userId);
        return await query.ToListAsync();
    }

    public async Task<List<string?>> GetRoleNamesAsync(string userId, string roleId = null, string tenantId = null)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); // Replace with your DbContext

        var query = dbContext.UserRoles.AsQueryable();
        if (!string.IsNullOrEmpty(tenantId))
            query = query.Where(role => role.TenantId == tenantId);
        if (!string.IsNullOrEmpty(roleId))
            query = query.Where(role => role.RoleId == roleId);
        if (!string.IsNullOrEmpty(userId))
            query = query.Where(role => role.UserId == userId);
        return await query.Select(x => x.Role.Name).ToListAsync();
    }
}

public class CustomRoleManager : RoleManager<ApplicationRole>
{
    public CustomRoleManager(
        IRoleStore<ApplicationRole> store,
        IEnumerable<IRoleValidator<ApplicationRole>> roleValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        ILogger<CustomRoleManager> logger)
        : base(store, roleValidators, keyNormalizer, errors, logger)
    {
    }
    public async Task<ApplicationRole> FindByNameAsync(string roleName, TenantType type)
    {
        return await FindByNameAsync(roleName, (byte)type);
    }
    public async Task<ApplicationRole> FindByNameAsync(string roleName, byte tenantType)
    {
        return await Roles?.FirstOrDefaultAsync(r => r.Name == roleName && r.TenantType == tenantType);
    }
    public async Task<ApplicationRole> AddToRoleAsync(ApplicationUser user, string roleName)
    {
        throw new NotImplementedException("Please use with tenantid");
    }

    //public async Task<IdentityRole> FindByNameAsync(string roleName)
    //{
    //    return await Roles?.FirstOrDefaultAsync(r => r.Name == roleName);
    //}
}
