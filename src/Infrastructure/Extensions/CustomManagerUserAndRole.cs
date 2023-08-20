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
        ILogger<UserManager<ApplicationUser>> logger)
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
        ILogger<RoleManager<ApplicationRole>> logger)
        : base(store, roleValidators, keyNormalizer, errors, logger)
    {
    }
}
