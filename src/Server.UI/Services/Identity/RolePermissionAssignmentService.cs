using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Common.Security;
using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Server.UI.Services.Identity;

/// <inheritdoc />
public class RolePermissionAssignmentService : IPermissionAssignmentService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly PermissionHelper _permissionHelper;
    private readonly IFusionCache _cache;
    private readonly ILogger<RolePermissionAssignmentService> _logger;
    private const string CacheKeyPrefix = "get-claims-by-";

    public RolePermissionAssignmentService(RoleManager<ApplicationRole> roleManager,
        PermissionHelper permissionHelper,
        IFusionCache cache,
        ILogger<RolePermissionAssignmentService> logger)
    {
        _roleManager = roleManager;
        _permissionHelper = permissionHelper;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IList<PermissionModel>> LoadAsync(string entityId)
    {
        return await _permissionHelper.GetAllPermissionsByRoleId(entityId);
    }

    public async Task AssignAsync(PermissionModel model)
    {
        var roleId = model.RoleId ?? throw new ArgumentNullException(nameof(model.RoleId));
        var role = await _roleManager.FindByIdAsync(roleId) ??
                   throw new NotFoundException($"Role not found: {roleId}");
        var claim = new Claim(model.ClaimType, model.ClaimValue);
        if (model.Assigned)
            await _roleManager.AddClaimAsync(role, claim);
        else
            await _roleManager.RemoveClaimAsync(role, claim);
        InvalidateCache(roleId);
    }

    public async Task AssignBulkAsync(IEnumerable<PermissionModel> models)
    {
        var list = models.ToList();
        if (!list.Any()) return;
        var roleId = list.First().RoleId ?? string.Empty;
        var role = await _roleManager.FindByIdAsync(roleId) ??
                   throw new NotFoundException($"Role not found: {roleId}");
        foreach (var model in list)
        {
            var claim = new Claim(model.ClaimType, model.ClaimValue);
            if (model.Assigned)
                await _roleManager.AddClaimAsync(role, claim);
            else
                await _roleManager.RemoveClaimAsync(role, claim);
        }
        InvalidateCache(roleId);
    }

    private void InvalidateCache(string roleId)
    {
        _cache.Remove($"{CacheKeyPrefix}{roleId}");
    }
} 