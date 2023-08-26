using CleanArchitecture.Blazor.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;

public class CustomUserManager : UserManager<ApplicationUser>
{
    public CustomUserManager(
        IUserStore<ApplicationUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<ApplicationUser> passwordHasher,
        IEnumerable<IUserValidator<ApplicationUser>> userValidators,
        IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<CustomUserManager> logger)
        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
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
        // Implement the logic to find the role by name and TenantId
        // You'll likely need to use your data store to perform the lookup
        return await Roles?.FirstOrDefaultAsync(r => r.Name == roleName && r.TenantType == tenantType);
    }
    public async Task<IdentityRole> FindByNameAsync(string roleName)
    {
        throw new NotImplementedException("Please use the method with tenantType");
    }
}
