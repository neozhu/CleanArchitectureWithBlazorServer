using System.ComponentModel;
using System.Reflection;
using System.Security.Claims;
using CleanArchitecture.Blazor.Application.Common.Constants;
using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Security;
using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

/// <summary>
/// Queries user and role permissions and maps them to <see cref="PermissionModel"/>.
/// </summary>
public class PermissionQueryService : IPermissionQueryService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public PermissionQueryService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<IList<PermissionModel>> GetAllPermissionsByUserId(string userId)
    {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        var assignedClaims = await GetUserClaimsByUserId(userManager, userId).ConfigureAwait(false);
        var inheritClaims = await GetInheritedClaims(userManager, roleManager, userId).ConfigureAwait(false);
        var combinedClaims = assignedClaims.Concat(inheritClaims).Distinct(new ClaimComparer()).ToList();

        return BuildUserPermissionModels(userId, combinedClaims, inheritClaims);
    }

    public async Task<IList<PermissionModel>> GetAllPermissionsByRoleId(string roleId)
    {
        using var scope = _scopeFactory.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        var assignedClaims = await GetRoleClaimsByRoleId(roleManager, roleId).ConfigureAwait(false);
        return BuildRolePermissionModels(roleId, assignedClaims);
    }

    private static async Task<IList<Claim>> GetUserClaimsByUserId(UserManager<ApplicationUser> userManager, string userId)
    {
        var user = await userManager.FindByIdAsync(userId).ConfigureAwait(false)
                   ?? throw new NotFoundException($"not found application user: {userId}");
        return await userManager.GetClaimsAsync(user).ConfigureAwait(false);
    }

    private static async Task<List<Claim>> GetInheritedClaims(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        string userId)
    {
        var user = await userManager.FindByIdAsync(userId).ConfigureAwait(false)
                   ?? throw new NotFoundException($"not found application user: {userId}");
        var roles = await userManager.GetRolesAsync(user).ConfigureAwait(false);
        var inheritedClaims = new List<Claim>();

        if (!roles.Any())
        {
            return inheritedClaims;
        }

        var assignedRoles = await roleManager.Roles
            .Where(role => role.Name != null && roles.Contains(role.Name))
            .ToListAsync()
            .ConfigureAwait(false);

        foreach (var role in assignedRoles)
        {
            var claims = await roleManager.GetClaimsAsync(role).ConfigureAwait(false);
            inheritedClaims.AddRange(claims);
        }

        return inheritedClaims.Distinct(new ClaimComparer()).ToList();
    }

    private static async Task<IList<Claim>> GetRoleClaimsByRoleId(RoleManager<ApplicationRole> roleManager, string roleId)
    {
        var role = await roleManager.FindByIdAsync(roleId).ConfigureAwait(false)
                   ?? throw new NotFoundException($"not found application role: {roleId}");
        return await roleManager.GetClaimsAsync(role).ConfigureAwait(false);
    }

    private IList<PermissionModel> BuildUserPermissionModels(string userId, IEnumerable<Claim> claimsToCheck, IEnumerable<Claim> inheritedClaims)
    {
        var result = new List<PermissionModel>();
        var inheritedClaimValues = new HashSet<string>(inheritedClaims.Select(claim => claim.Value));
        var assignedClaimValues = new HashSet<string>(claimsToCheck.Select(claim => claim.Value));

        foreach (var (moduleInfo, field) in GetPermissionFields())
        {
            var claimValue = field.GetValue(null)?.ToString();
            if (string.IsNullOrEmpty(claimValue))
            {
                continue;
            }

            var isInherited = inheritedClaimValues.Contains(claimValue);
            result.Add(new PermissionModel
            {
                UserId = userId,
                ClaimValue = claimValue,
                ClaimType = ApplicationClaimTypes.Permission,
                Group = moduleInfo.DisplayName,
                Name = FormatFieldNameForDisplay(field.Name),
                HelpText = isInherited
                    ? "This permission is inherited and cannot be modified."
                    : field.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault()?.Description ?? string.Empty,
                Description = moduleInfo.Description,
                Assigned = assignedClaimValues.Contains(claimValue),
                IsInherit = isInherited
            });
        }

        return result;
    }

    private IList<PermissionModel> BuildRolePermissionModels(string roleId, IEnumerable<Claim> assignedClaims)
    {
        var result = new List<PermissionModel>();
        var assignedClaimValues = new HashSet<string>(assignedClaims.Select(claim => claim.Value));

        foreach (var (moduleInfo, field) in GetPermissionFields())
        {
            var claimValue = field.GetValue(null)?.ToString();
            if (string.IsNullOrEmpty(claimValue))
            {
                continue;
            }

            result.Add(new PermissionModel
            {
                RoleId = roleId,
                ClaimValue = claimValue,
                ClaimType = ApplicationClaimTypes.Permission,
                Group = moduleInfo.DisplayName,
                Name = FormatFieldNameForDisplay(field.Name),
                HelpText = field.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault()?.Description ?? string.Empty,
                Description = moduleInfo.Description,
                Assigned = assignedClaimValues.Contains(claimValue)
            });
        }

        return result;
    }

    private static IEnumerable<(ModuleInfo ModuleInfo, FieldInfo Field)> GetPermissionFields()
    {
        foreach (var module in typeof(Permissions).GetNestedTypes())
        {
            var displayNameAttribute = module.GetCustomAttributes<DisplayNameAttribute>().FirstOrDefault();
            var descriptionAttribute = module.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault();
            var moduleInfo = new ModuleInfo(
                displayNameAttribute?.DisplayName ?? string.Empty,
                descriptionAttribute?.Description ?? string.Empty);

            foreach (var field in module.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                yield return (moduleInfo, field);
            }
        }
    }

    private static string FormatFieldNameForDisplay(string fieldName)
    {
        var name = System.Text.RegularExpressions.Regex.Replace(fieldName, "(\\B[A-Z])", " $1").Trim().ToLower();
        return char.ToUpper(name[0]) + name[1..];
    }

    private sealed class ClaimComparer : IEqualityComparer<Claim>
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