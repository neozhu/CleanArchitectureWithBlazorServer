using CleanArchitecture.Blazor.Application.Common.Security;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
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

public class PermissionHelper
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IFusionCache _fusionCache;
    private readonly TimeSpan _refreshInterval;

    public PermissionHelper(IServiceScopeFactory scopeFactory, IFusionCache fusionCache)
    {
        var scope = scopeFactory.CreateScope();
        _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        _fusionCache = fusionCache;
        _refreshInterval = TimeSpan.FromDays(1);
    }

    public async Task<IList<PermissionModel>> GetAllPermissionsByUserId(string userId)
    {
        var assignedClaims = await GetUserClaimsByUserId(userId).ConfigureAwait(false);
        IList<PermissionModel> allPermissions = new List<PermissionModel>();
        var modules = typeof(Permissions).GetNestedTypes();

        foreach (var module in modules)
        {
            var moduleName = module.GetCustomAttributes<DisplayNameAttribute>().FirstOrDefault()?.DisplayName ?? string.Empty;
            var moduleDescription = module.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault()?.Description ?? string.Empty;
            var fields = module.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            allPermissions = allPermissions.Concat(fields.Select(field => field.GetValue(null)?.ToString())
                .Where(claimValue => claimValue != null)
                .Select(claimValue => new PermissionModel
                {
                    UserId = userId,
                    ClaimValue = claimValue ?? string.Empty,
                    ClaimType = ApplicationClaimTypes.Permission,
                    Group = moduleName,
                    Description = moduleDescription,
                    Assigned = assignedClaims.Any(x => x.Value.Equals(claimValue))
                })).ToList();
        }

        return allPermissions;
    }

    private async Task<IList<Claim>> GetUserClaimsByUserId(string userId)
    {
        var key = $"get-claims-by-{userId}";
        return await _fusionCache.GetOrSetAsync(key, async _ =>
        {
            var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false)
                       ?? throw new NotFoundException($"not found application user: {userId}");
            return await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
        }, _refreshInterval).ConfigureAwait(false);
    }

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

            allPermissions = allPermissions.Concat(fields.Select(field => field.GetValue(null)?.ToString())
                .Where(claimValue => !string.IsNullOrEmpty(claimValue))
                .Select(claimValue => new PermissionModel
                {
                    RoleId = roleId,
                    ClaimValue = claimValue ?? string.Empty,
                    ClaimType = ApplicationClaimTypes.Permission,
                    Group = moduleName,
                    Description = moduleDescription,
                    Assigned = assignedClaims.Any(x => x.Value.Equals(claimValue))
                })).ToList();
        }

        return allPermissions;
    }

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
}
