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

public class RoleHelper
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IFusionCache _fusionCache;
    private readonly TimeSpan _refreshInterval;

    public RoleHelper(RoleManager<ApplicationRole> roleManager, IFusionCache fusionCache)
    {
        _roleManager = roleManager;
        _fusionCache = fusionCache;
        _refreshInterval = TimeSpan.FromDays(1);
    }
    public async Task<List<PermissionModel>> GetAllPermissions(ApplicationRoleDto dto)
    {
        var assignedClaims = await GetUserClaims(dto.Id);
        var allPermissions = new List<PermissionModel>();
        var modules = typeof(Permissions).GetNestedTypes();

        foreach (var module in modules)
        {
            var moduleName = module.GetCustomAttributes<DisplayNameAttribute>().FirstOrDefault()?.DisplayName ?? string.Empty;
            var moduleDescription = module.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault()?.Description ?? string.Empty;
            var fields = module.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            allPermissions.AddRange(fields.Select(field => field.GetValue(null)?.ToString())
                .Where(claimValue => claimValue != null)
                .Select(claimValue => new PermissionModel
                {
                    RoleId = dto.Id,
                    ClaimValue = claimValue ?? string.Empty,
                    ClaimType = ApplicationClaimTypes.Permission,
                    Group = moduleName,
                    Description = moduleDescription,
                    Assigned = assignedClaims.Any(x => x.Value == claimValue)
                }));
        }

        return allPermissions;
    }
    private async Task<IList<Claim>> GetUserClaims(string roleId)
    {
        var key = $"get-claims-by-{roleId}";
        return await _fusionCache.GetOrSetAsync(key, async _ =>
        {
            var role = await _roleManager.FindByIdAsync(roleId) ?? throw new NotFoundException($"not found application role: {roleId}");
            return await _roleManager.GetClaimsAsync(role);
        }, _refreshInterval);
    }
}

