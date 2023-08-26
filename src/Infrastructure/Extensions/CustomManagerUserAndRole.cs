using CleanArchitecture.Blazor.Application.Constants.Role;
using CleanArchitecture.Blazor.Domain.Enums;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;

public class CustomUserManager : UserManager<ApplicationUser>
{
    public const string defaultTenantId = "";//todo make it loaded as per db
    private readonly CustomRoleManager _roleManager;
    private readonly IUserRoleStore<ApplicationUser> _userRoleStore;
    private readonly IUserStore<ApplicationUser> _store;
    public CustomUserManager(
         IUserStore<ApplicationUser> store,
         IOptions<IdentityOptions> optionsAccessor,
         IPasswordHasher<ApplicationUser> passwordHasher,
         IEnumerable<IUserValidator<ApplicationUser>> userValidators,
         IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
         ILookupNormalizer keyNormalizer,
         IdentityErrorDescriber errors,
         IServiceProvider services,
         ILogger<UserManager<ApplicationUser>> logger, CustomRoleManager roleManager, IUserRoleStore<ApplicationUser> userRoleStore)
         : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        _userRoleStore = userRoleStore;
        _store = store;
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
        var currentRoles = await GetRolesAsync(user, tenantId);

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

        return IdentityResult.Success;
    }

    private async Task<IList<string>> GetRolesAsync(ApplicationUser user, string tenantId)
    {
        var rolesForTenant = await _userRoleStore.GetRolesAsync(user,CancellationToken.None);
        return rolesForTenant;
        // Implement logic to retrieve user's roles for the given tenant
        // This might involve querying your data store for the roles

        // Example pseudocode:
        // return await _userRoleStore.GetRolesAsync(user, tenantId);
        // Ensure you have implemented the custom user role store
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
