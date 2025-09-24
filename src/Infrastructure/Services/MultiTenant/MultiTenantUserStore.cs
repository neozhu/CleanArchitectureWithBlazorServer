using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Infrastructure.Services.MultiTenant;

/// <summary>
/// Represents a multi-tenant user store.
/// </summary>
public class MultiTenantUserStore : UserStore<
    ApplicationUser,
    ApplicationRole,
    ApplicationDbContext,
    string,
    ApplicationUserClaim,
    ApplicationUserRole,
    ApplicationUserLogin,
    ApplicationUserToken,
    ApplicationRoleClaim>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MultiTenantUserStore"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public MultiTenantUserStore(ApplicationDbContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Adds a user to a role asynchronously.
    /// </summary>
    /// <param name="user">The user to add to the role.</param>
    /// <param name="normalizedRoleName">The normalized name of the role.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task AddToRoleAsync(ApplicationUser user, string normalizedRoleName, CancellationToken cancellationToken = default)
    {
        // Check if the operation has been canceled
        cancellationToken.ThrowIfCancellationRequested();
        // Ensure the object has not been disposed before proceeding
        ThrowIfDisposed();

        // Validate the user and role name parameters
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(normalizedRoleName)) throw new ArgumentException("Value cannot be null or empty.", nameof(normalizedRoleName));

        // Retrieve the role entity for the given tenant and role name
        var roleEntity = await GetRoleAsync(normalizedRoleName, user.TenantId??string.Empty, cancellationToken);
        if (roleEntity == null) throw new InvalidOperationException($"Role '{normalizedRoleName}' does not exist in the user's tenant.");

        // Check if the user is already assigned to the role
        if (await IsUserInRoleAsync(user.Id, roleEntity.Id, cancellationToken)) return;

        // Add the user-role relationship to the context
        Context.UserRoles.Add(new ApplicationUserRole
        {
            UserId = user.Id,
            RoleId = roleEntity.Id
        });
    }

    /// <summary>
    /// Checks if a user is in a role asynchronously.
    /// </summary>
    /// <param name="user">The user to check.</param>
    /// <param name="normalizedRoleName">The normalized name of the role.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task<bool> IsInRoleAsync(ApplicationUser user, string normalizedRoleName, CancellationToken cancellationToken = default)
    {
        // Check if the operation has been canceled
        cancellationToken.ThrowIfCancellationRequested();
        // Ensure the object has not been disposed before proceeding
        ThrowIfDisposed();

        // Validate the user and role name parameters
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(normalizedRoleName)) throw new ArgumentException("Value cannot be null or empty.", nameof(normalizedRoleName));

        // Retrieve the role entity for the given tenant and role name
        var role = await GetRoleAsync(normalizedRoleName, user.TenantId ?? string.Empty, cancellationToken);
        // Check if the user is in the role
        return role != null && await IsUserInRoleAsync(user.Id, role.Id, cancellationToken);
    }

    /// <summary>
    /// Removes a user from a role asynchronously.
    /// </summary>
    /// <param name="user">The user to remove from the role.</param>
    /// <param name="normalizedRoleName">The normalized name of the role.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task RemoveFromRoleAsync(ApplicationUser user, string normalizedRoleName, CancellationToken cancellationToken = default)
    {
        // Check if the operation has been canceled
        cancellationToken.ThrowIfCancellationRequested();
        // Ensure the object has not been disposed before proceeding
        ThrowIfDisposed();

        // Validate the user and role name parameters
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(normalizedRoleName)) throw new ArgumentException("Value cannot be null or empty.", nameof(normalizedRoleName));

        // Retrieve the role entity for the given tenant and role name
        var role = await GetRoleAsync(normalizedRoleName, user.TenantId ?? string.Empty, cancellationToken);
        if (role != null)
        {
            // Retrieve the user-role relationship for the given user and role
            var userRole = await GetUserRoleAsync(user.Id, role.Id, cancellationToken);
            if (userRole != null)
            {
                // Remove the user-role relationship from the context
                Context.UserRoles.Remove(userRole);
            }
        }
    }

    // Retrieve a role entity based on the normalized role name and tenant ID
    private  Task<ApplicationRole?> GetRoleAsync(string normalizedRoleName, string tenantId, CancellationToken cancellationToken)
    {
        return  Context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == normalizedRoleName && r.TenantId == tenantId, cancellationToken);
    }

    // Retrieve a user-role relationship based on user ID and role ID
    private  Task<ApplicationUserRole?> GetUserRoleAsync(string userId, string roleId, CancellationToken cancellationToken)
    {
        return  Context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
    }

    // Check if a user is in a given role
    private  Task<bool> IsUserInRoleAsync(string userId, string roleId, CancellationToken cancellationToken)
    {
        return  Context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
    }
}
