using CleanArchitecture.Blazor.Application.Common.Security;
using System.ComponentModel;
using System.Reflection;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Application.Common.Constants.ClaimTypes;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using ZiggyCreatures.Caching.Fusion;
using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

namespace CleanArchitecture.Blazor.Server.UI.Services;

/// <summary>
/// Helper class for managing permissions.
/// </summary>
public class PermissionHelper : IPermissionHelper, IDisposable
{
    private readonly IFusionCache _fusionCache;
    private readonly TimeSpan _refreshInterval;
    private readonly IServiceScopeFactory _scopeFactory;
    private const string ClaimsCacheKeyPrefix = "get-claims-by-";

    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionHelper"/> class.
    /// </summary>
    /// <param name="scopeFactory">The service scope factory.</param>
    /// <param name="fusionCache">The fusion cache.</param>
    public PermissionHelper(IServiceScopeFactory scopeFactory, IFusionCache fusionCache)
    {
        _scopeFactory = scopeFactory;
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
        
        return await BuildPermissionModels(
            moduleType: typeof(Permissions),
            claimsToCheck: combinedClaims,
            additionalParams: new Dictionary<string, object>
            {
                { "userId", userId },
                { "inheritClaims", inheritClaims }
            },
            permissionModelFactory: (moduleInfo, fieldInfo, claimsToCheck, additionalParams) =>
            {
                var claimValue = fieldInfo.GetValue(null)?.ToString();
                var userId = (string)additionalParams["userId"];
                var inheritClaims = (IList<Claim>)additionalParams["inheritClaims"];
                
                // Convert field name from PascalCase/CamelCase to space-separated words
                var name = FormatFieldNameForDisplay(fieldInfo.Name);
                
                var isInherited = inheritClaims.Any(x => x.Value.Equals(claimValue));
                var helpText = isInherited
                    ? "This permission is inherited and cannot be modified."
                    : fieldInfo.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault()?.Description ?? string.Empty;

                return new PermissionModel
                {
                    UserId = userId,
                    ClaimValue = claimValue ?? string.Empty,
                    ClaimType = ApplicationClaimTypes.Permission,
                    Group = moduleInfo.DisplayName,
                    Name = name,
                    HelpText = helpText,
                    Description = moduleInfo.Description,
                    Assigned = claimsToCheck.Any(x => x.Value.Equals(claimValue)),
                    IsInherit = isInherited
                };
            }
        );
    }
    
    /// <summary>
    /// Formats a field name from PascalCase to a more readable format with spaces
    /// </summary>
    private static string FormatFieldNameForDisplay(string fieldName)
    {
        var name = System.Text.RegularExpressions.Regex.Replace(fieldName, "(\\B[A-Z])", " $1").Trim().ToLower();
        return char.ToUpper(name[0]) + name.Substring(1); // Capitalize the first letter
    }

    private async Task<List<Claim>> GetInheritedClaims(string userId)
    {
        var key = $"get-inherited-claims-by-{userId}";
        return await _fusionCache.GetOrSetAsync(key, async _ =>
        {
            using var scope = _scopeFactory.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            
            var user = await userManager.FindByIdAsync(userId).ConfigureAwait(false)
                                    ?? throw new NotFoundException($"not found application user: {userId}");
            var roles = (await userManager.GetRolesAsync(user)).ToArray();
            var inheritClaims = new List<Claim>();
            if (roles is not null && roles.Any())
            {
                var assigendRoles = await roleManager.Roles.Where(x => roles.Contains(x.Name) && x.TenantId == user.TenantId).ToListAsync();
                foreach (var role in assigendRoles)
                {
                    var claims = await roleManager.GetClaimsAsync(role).ConfigureAwait(false);
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
        var key = $"{ClaimsCacheKeyPrefix}{userId}";
        return await _fusionCache.GetOrSetAsync(key, async _ =>
        {
            using var scope = _scopeFactory.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            
            var user = await userManager.FindByIdAsync(userId).ConfigureAwait(false)
                       ?? throw new NotFoundException($"not found application user: {userId}");
            var userClaims = await userManager.GetClaimsAsync(user).ConfigureAwait(false);
            return userClaims;

        }, _refreshInterval).ConfigureAwait(false);    }
    
    /// <summary>
    /// Gets all permissions for a role by role ID.
    /// </summary>
    /// <param name="roleId">The role ID.</param>
    /// <returns>The list of permission models.</returns>
    public async Task<IList<PermissionModel>> GetAllPermissionsByRoleId(string roleId)
    {
        var assignedClaims = await GetUserClaimsByRoleId(roleId).ConfigureAwait(false);
        
        return await BuildPermissionModels(
            moduleType: typeof(Permissions),
            claimsToCheck: assignedClaims,
            additionalParams: new Dictionary<string, object>
            {
                { "roleId", roleId }
            },
            permissionModelFactory: (moduleInfo, fieldInfo, claimsToCheck, additionalParams) =>
            {
                var claimValue = fieldInfo.GetValue(null)?.ToString();
                var roleId = (string)additionalParams["roleId"];
                
                // Convert field name from PascalCase/CamelCase to space-separated words
                var name = FormatFieldNameForDisplay(fieldInfo.Name);
                
                var helpText = fieldInfo.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault()?.Description ?? string.Empty;

                return new PermissionModel
                {
                    RoleId = roleId,
                    ClaimValue = claimValue ?? string.Empty,
                    ClaimType = ApplicationClaimTypes.Permission,
                    Group = moduleInfo.DisplayName,
                    Name = name,
                    HelpText = helpText,
                    Description = moduleInfo.Description,
                    Assigned = claimsToCheck.Any(x => x.Value.Equals(claimValue))
                };
            }
        );
    }

    /// <summary>
    /// Gets the user claims by role ID.
    /// </summary>
    /// <param name="roleId">The role ID.</param>
    /// <returns>The list of claims.</returns>
    private async Task<IList<Claim>> GetUserClaimsByRoleId(string roleId)
    {
        var key = $"{ClaimsCacheKeyPrefix}{roleId}";
        return await _fusionCache.GetOrSetAsync(key, async _ =>
        {
            using var scope = _scopeFactory.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            
            var role = await roleManager.FindByIdAsync(roleId).ConfigureAwait(false)
                       ?? throw new NotFoundException($"not found application role: {roleId}");
            return await roleManager.GetClaimsAsync(role).ConfigureAwait(false);
        }, _refreshInterval).ConfigureAwait(false);    }
    
    /// <summary>
    /// Builds permission models from a given module type and claims.
    /// </summary>
    /// <param name="moduleType">The module type to extract permissions from.</param>
    /// <param name="claimsToCheck">The claims to check for permission assignments.</param>
    /// <param name="additionalParams">Additional parameters for the factory method.</param>
    /// <param name="permissionModelFactory">Factory method to create permission models.</param>
    /// <returns>A list of permission models.</returns>
    private async Task<IList<PermissionModel>> BuildPermissionModels(
        Type moduleType,
        IEnumerable<Claim> claimsToCheck,
        Dictionary<string, object> additionalParams,
        Func<ModuleInfo, FieldInfo, IEnumerable<Claim>, Dictionary<string, object>, PermissionModel> permissionModelFactory)
    {
        var result = new List<PermissionModel>();
        var modules = moduleType.GetNestedTypes();

        foreach (var module in modules)
        {
            var displayNameAttribute = module.GetCustomAttributes<DisplayNameAttribute>().FirstOrDefault();
            var descriptionAttribute = module.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault();
            
            var moduleInfo = new ModuleInfo(
                displayNameAttribute?.DisplayName ?? string.Empty,
                descriptionAttribute?.Description ?? string.Empty
            );
            
            var fields = module.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            foreach (var field in fields)
            {
                var claimValue = field.GetValue(null)?.ToString();
                if (string.IsNullOrEmpty(claimValue))
                {
                    continue;
                }

                var model = permissionModelFactory(
                    moduleInfo,
                    field,
                    claimsToCheck,
                    additionalParams
                );

                result.Add(model);
            }
        }

        return await Task.FromResult(result);
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

    public void Dispose()
    {
        // No long-lived resources to dispose
        GC.SuppressFinalize(this);
    }
}
