using CleanArchitecture.Blazor.Application.Common.Security;
using System.ComponentModel;
using System.Reflection;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Constants.ClaimTypes;
using CleanArchitecture.Blazor.Infrastructure.PermissionSet;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using ZiggyCreatures.Caching.Fusion;
using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
 

namespace CleanArchitecture.Blazor.Server.UI.Services;


/// <summary>
/// Helper class for managing permissions.
/// </summary>
public class PermissionHelper
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IFusionCache _fusionCache;
    private readonly TimeSpan _refreshInterval;

    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionHelper"/> class.
    /// </summary>
    /// <param name="scopeFactory">The service scope factory.</param>
    /// <param name="fusionCache">The fusion cache.</param>
    public PermissionHelper(IServiceScopeFactory scopeFactory, IFusionCache fusionCache)
    {
        var scope = scopeFactory.CreateScope();
        _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        _fusionCache = fusionCache;
        _refreshInterval = TimeSpan.FromSeconds(30);
    }

    /// <summary>
    /// Gets all permissions for a user by user ID.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>The list of permission models.</returns>
    public async Task<IList<PermissionModel>> GetAllPermissionsByUserId(string userId)
    {
        var assignedClaims = await GetUserClaimsByUserId(userId).ConfigureAwait(false);
        var inheritClaims = await GetInheritedClaims(userId).ConfigureAwait(false);
        var combinedClaims = assignedClaims.Concat(inheritClaims).Distinct(new ClaimComparer()).ToList();
        IList<PermissionModel> allPermissions = new List<PermissionModel>();
        var modules = typeof(Permissions).GetNestedTypes();

        foreach (var module in modules)
        {
            var moduleName = module.GetCustomAttributes<DisplayNameAttribute>().FirstOrDefault()?.DisplayName ?? string.Empty;
            var moduleDescription = module.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault()?.Description ?? string.Empty;
            var fields = module.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            allPermissions = allPermissions.Concat(fields.Select(field =>
            {
                var claimValue = field.GetValue(null)?.ToString();
                // Convert field name from PascalCase/CamelCase to space-separated words with first letter capitalized
                var name = System.Text.RegularExpressions.Regex.Replace(field.Name, "(\\B[A-Z])", " $1").Trim().ToLower();
                name = char.ToUpper(name[0]) + name.Substring(1); // Capitalize the first letter
                var helpText = inheritClaims.Any(x => x.Value.Equals(claimValue))
                ? "This permission is inherited and cannot be modified."
                : field.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault()?.Description ?? string.Empty;


                return new PermissionModel
                {
                    UserId = userId,
                    ClaimValue = claimValue ?? string.Empty,
                    ClaimType = ApplicationClaimTypes.Permission,
                    Group = moduleName,
                    Name = name, // Assigning the field name
                    HelpText = helpText, // Assigning the description as HelpText
                    Description = moduleDescription,
                    Assigned = combinedClaims.Any(x => x.Value.Equals(claimValue)),
                    IsInherit = inheritClaims.Any(x => x.Value.Equals(claimValue))

                };
            }).Where(pm => !string.IsNullOrEmpty(pm.ClaimValue))).ToList();
        }

        return allPermissions;
    }

    private async Task<List<Claim>> GetInheritedClaims(string userId)
    {
        var key = $"get-inherited-claims-by-{userId}";
        return await _fusionCache.GetOrSetAsync(key, async _ =>
        {
            var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false)
                                    ?? throw new NotFoundException($"not found application user: {userId}");
            var roles = (await _userManager.GetRolesAsync(user)).ToArray();
            var inheritClaims = new List<Claim>();
            if (roles is not null && roles.Any())
            {
                var assigendRoles = await _roleManager.Roles.Where(x => roles.Contains(x.Name) && x.TenantId == user.TenantId).ToListAsync();
                foreach (var role in assigendRoles)
                {
                    var claims = await _roleManager.GetClaimsAsync(role).ConfigureAwait(false);
                    inheritClaims.AddRange(claims);
                }
                inheritClaims = inheritClaims.Distinct(new ClaimComparer()).ToList();
            }

            return inheritClaims;
        }, _refreshInterval).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the user claims by user ID.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>The list of claims.</returns>
    private async Task<IList<Claim>> GetUserClaimsByUserId(string userId)
    {
        var key = $"get-claims-by-{userId}";
        return await _fusionCache.GetOrSetAsync(key, async _ =>
        {
            var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false)
                       ?? throw new NotFoundException($"not found application user: {userId}");
            var userClaims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
            return userClaims;

        }, _refreshInterval).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets all permissions for a role by role ID.
    /// </summary>
    /// <param name="roleId">The role ID.</param>
    /// <returns>The list of permission models.</returns>
    public async Task<IList<PermissionModel>> GetAllPermissionsByRoleId(string roleId)
    {
        var assignedClaims = await GetUserClaimsByRoleId(roleId).ConfigureAwait(false);
        IList<PermissionModel> allPermissions = new List<PermissionModel>();
        var modules = typeof(Permissions).GetNestedTypes();

        foreach (var module in modules)
        {
            var moduleName = module.GetCustomAttributes<DisplayNameAttribute>().FirstOrDefault()?.DisplayName ?? string.Empty;
            var moduleDescription = module.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault()?.Description ?? string.Empty;
            var fields = module.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            allPermissions = allPermissions.Concat(fields.Select(field =>
            {
                var claimValue = field.GetValue(null)?.ToString();
                // Convert field name from PascalCase/CamelCase to space-separated words with first letter capitalized
                var name = System.Text.RegularExpressions.Regex.Replace(field.Name, "(\\B[A-Z])", " $1").Trim().ToLower();
                name = char.ToUpper(name[0]) + name.Substring(1); // Capitalize the first letter
                var helpText = field.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault()?.Description ?? string.Empty; // Get the description attribute

                return new PermissionModel
                {
                    RoleId = roleId,
                    ClaimValue = claimValue ?? string.Empty,
                    ClaimType = ApplicationClaimTypes.Permission,
                    Group = moduleName,
                    Name = name, // Assigning the field name
                    HelpText = helpText, // Assigning the description as HelpText
                    Description = moduleDescription,
                    Assigned = assignedClaims.Any(x => x.Value.Equals(claimValue))
                };
            }).Where(pm => !string.IsNullOrEmpty(pm.ClaimValue))).ToList();
        }

        return allPermissions;
    }

    /// <summary>
    /// Gets the user claims by role ID.
    /// </summary>
    /// <param name="roleId">The role ID.</param>
    /// <returns>The list of claims.</returns>
    private async Task<IList<Claim>> GetUserClaimsByRoleId(string roleId)
    {
        var key = $"get-claims-by-{roleId}";
        return await _fusionCache.GetOrSetAsync(key, async _ =>
        {
            var role = await _roleManager.FindByIdAsync(roleId).ConfigureAwait(false)
                       ?? throw new NotFoundException($"not found application role: {roleId}");
            return await _roleManager.GetClaimsAsync(role).ConfigureAwait(false);
        }, _refreshInterval).ConfigureAwait(false);
    }

    /// <summary>
    /// Custom comparer for claims.
    /// </summary>
    public class ClaimComparer : IEqualityComparer<Claim>
    {
        public bool Equals(Claim? x, Claim? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            return x.Type.Equals(y.Type) && x.Value.Equals(y.Value);
        }

        public int GetHashCode(Claim obj)
        {
            return HashCode.Combine(obj.Type, obj.Value);
        }
    }
}
