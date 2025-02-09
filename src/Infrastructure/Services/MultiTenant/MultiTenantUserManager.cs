using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Infrastructure.Services.MultiTenant;

/// <summary>
/// Custom user manager implementation for multi-tenant application.
/// </summary>
public class MultiTenantUserManager : UserManager<ApplicationUser>
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiTenantUserManager"/> class.
    /// </summary>
    /// <param name="store">The user store.</param>
    /// <param name="optionsAccessor">The identity options accessor.</param>
    /// <param name="passwordHasher">The password hasher.</param>
    /// <param name="userValidators">The user validators.</param>
    /// <param name="passwordValidators">The password validators.</param>
    /// <param name="keyNormalizer">The key normalizer.</param>
    /// <param name="errors">The identity error describer.</param>
    /// <param name="services">The service provider.</param>
    /// <param name="roleManager">The role manager.</param>
    /// <param name="logger">The logger.</param>
    public MultiTenantUserManager(
        IUserStore<ApplicationUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<ApplicationUser> passwordHasher,
        IEnumerable<IUserValidator<ApplicationUser>> userValidators,
        IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        RoleManager<ApplicationRole> roleManager,
        ILogger<UserManager<ApplicationUser>> logger)
        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        _roleManager = roleManager;
    }

    /// <summary>
    /// Adds the specified user to the given roles.
    /// </summary>
    /// <param name="user">The user to add to roles.</param>
    /// <param name="roles">The roles to add the user to.</param>
    /// <returns>The <see cref="IdentityResult"/> of the operation.</returns>
    public override async Task<IdentityResult> AddToRolesAsync(ApplicationUser user, IEnumerable<string> roles)
    {
        var tenantId = user.TenantId;
        var normalizedRoleNames = roles.Select(NormalizeName).ToList();

        var tenantRoles = await _roleManager.Roles
            .Where(r => normalizedRoleNames.Contains(r.NormalizedName) && r.TenantId == tenantId)
            .ToListAsync();

        if (tenantRoles.Count != roles.Count())
        {
            var missingRoles = roles.Except(tenantRoles.Select(r => r.Name), StringComparer.OrdinalIgnoreCase);
            return IdentityResult.Failed(new IdentityError
            {
                Code = "RoleNotFound",
                Description = $"Roles '{string.Join(", ", missingRoles)}' do not exist in the user's tenant."
            });
        }

        foreach (var role in tenantRoles)
        {
            var result = await AddToRoleAsync(user, role.Name);
            if (!result.Succeeded)
            {
                return result;
            }
        }

        return IdentityResult.Success;
    }

    /// <summary>
    /// Adds the specified user to the given role.
    /// </summary>
    /// <param name="user">The user to add to the role.</param>
    /// <param name="roleName">The name of the role to add the user to.</param>
    /// <returns>The <see cref="IdentityResult"/> of the operation.</returns>
    public override async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string roleName)
    {
        var tenantId = user.TenantId;
        var normalizedRoleName = NormalizeName(roleName);

        var role = await _roleManager.Roles
            .FirstOrDefaultAsync(r => r.NormalizedName == normalizedRoleName && r.TenantId == tenantId);

        if (role == null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "RoleNotFound",
                Description = $"Role '{roleName}' does not exist in the user's tenant."
            });
        }

        if (await IsInRoleAsync(user, role.Name!))
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "UserAlreadyInRole",
                Description = $"User is already in role '{roleName}'."
            });
        }

        var userRoleStore = GetUserRoleStore();
        await userRoleStore.AddToRoleAsync(user, role.NormalizedName!, CancellationToken.None);

        return await UpdateUserAsync(user);
    }

    /// <summary>
    /// Checks if the specified user is in the given role.
    /// </summary>
    /// <param name="user">The user to check.</param>
    /// <param name="roleName">The name of the role to check.</param>
    /// <returns><c>true</c> if the user is in the role, otherwise <c>false</c>.</returns>
    public override async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrEmpty(roleName)) throw new ArgumentException("Value cannot be null or empty.", nameof(roleName));

        var normalizedRoleName = NormalizeName(roleName);
        return await _roleManager.Roles.AnyAsync(r =>
            r.NormalizedName == normalizedRoleName &&
            r.TenantId == user.TenantId &&
            Context.UserRoles.Any(ur => ur.UserId == user.Id && ur.RoleId == r.Id));
    }
    

    /// <summary>
    /// Removes the specified user from the given role.
    /// </summary>
    /// <param name="user">The user to remove from the role.</param>
    /// <param name="role">The name of the role to remove the user from.</param>
    /// <returns>The <see cref="IdentityResult"/> of the operation.</returns>
    public override async Task<IdentityResult> RemoveFromRoleAsync(ApplicationUser user, string role)
    {
        var normalizedRoleName = NormalizeName(role);
        var userRoleStore = GetUserRoleStore();
        await userRoleStore.RemoveFromRoleAsync(user, normalizedRoleName, CancellationToken.None);

        return await UpdateUserAsync(user);
    }

    private IUserRoleStore<ApplicationUser> GetUserRoleStore()
    {
        return Store as IUserRoleStore<ApplicationUser>
               ?? throw new NotSupportedException("The user store does not implement IUserRoleStore<ApplicationUser>.");
    }

    private ApplicationDbContext Context => (Store as UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, string, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationUserToken, ApplicationRoleClaim>)?.Context ?? throw new InvalidOperationException("Context is not available.");
}
