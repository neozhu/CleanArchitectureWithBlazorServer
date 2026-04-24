using System.Security.Claims;
using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Security;
using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

public class PermissionAssignmentService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IPermissionQueryService _permissionQueryService;
    private readonly ILogger<PermissionAssignmentService> _logger;

    public PermissionAssignmentService(
        IServiceScopeFactory scopeFactory,
        IPermissionQueryService permissionQueryService,
        ILogger<PermissionAssignmentService> logger)
    {
        _scopeFactory = scopeFactory;
        _permissionQueryService = permissionQueryService;
        _logger = logger;
    }

    public Task<IList<PermissionModel>> LoadUserPermissionsAsync(string userId)
    {
        return _permissionQueryService.GetAllPermissionsByUserId(userId);
    }

    public Task<IList<PermissionModel>> LoadRolePermissionsAsync(string roleId)
    {
        return _permissionQueryService.GetAllPermissionsByRoleId(roleId);
    }

    public async Task AssignUserAsync(PermissionModel model)
    {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var userId = model.UserId ?? throw new ArgumentNullException(nameof(model.UserId));
        var user = await userManager.FindByIdAsync(userId)
                   ?? throw new NotFoundException($"User not found: {userId}");

        var claim = new Claim(model.ClaimType, model.ClaimValue);
        var result = model.Assigned
            ? await userManager.AddClaimAsync(user, claim)
            : await userManager.RemoveClaimAsync(user, claim);

        EnsureSucceeded(result, "update user permission", userId);
    }

    public async Task AssignRoleAsync(PermissionModel model)
    {
        using var scope = _scopeFactory.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        var roleId = model.RoleId ?? throw new ArgumentNullException(nameof(model.RoleId));
        var role = await roleManager.FindByIdAsync(roleId)
                   ?? throw new NotFoundException($"Role not found: {roleId}");

        var claim = new Claim(model.ClaimType, model.ClaimValue);
        var result = model.Assigned
            ? await roleManager.AddClaimAsync(role, claim)
            : await roleManager.RemoveClaimAsync(role, claim);

        EnsureSucceeded(result, "update role permission", roleId);
    }

    public async Task AssignUserBulkAsync(IEnumerable<PermissionModel> models)
    {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var list = models.ToList();
        if (!list.Any())
        {
            return;
        }

        var userId = GetSingleUserId(list);
        var user = await userManager.FindByIdAsync(userId)
                   ?? throw new NotFoundException($"User not found: {userId}");

        foreach (var model in list)
        {
            var claim = new Claim(model.ClaimType, model.ClaimValue);
            var result = model.Assigned
                ? await userManager.AddClaimAsync(user, claim)
                : await userManager.RemoveClaimAsync(user, claim);

            EnsureSucceeded(result, "update user permission", userId);
        }
    }

    public async Task AssignRoleBulkAsync(IEnumerable<PermissionModel> models)
    {
        using var scope = _scopeFactory.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        var list = models.ToList();
        if (!list.Any())
        {
            return;
        }

        var roleId = GetSingleRoleId(list);
        var role = await roleManager.FindByIdAsync(roleId)
                   ?? throw new NotFoundException($"Role not found: {roleId}");

        foreach (var model in list)
        {
            var claim = new Claim(model.ClaimType, model.ClaimValue);
            var result = model.Assigned
                ? await roleManager.AddClaimAsync(role, claim)
                : await roleManager.RemoveClaimAsync(role, claim);

            EnsureSucceeded(result, "update role permission", roleId);
        }
    }

    private static string GetSingleUserId(IReadOnlyCollection<PermissionModel> models)
    {
        var userIds = models.Select(model => model.UserId)
            .Where(userId => !string.IsNullOrWhiteSpace(userId))
            .Distinct()
            .ToList();

        return userIds.Count switch
        {
            1 => userIds[0]!,
            0 => throw new ArgumentException("Permission models must include a user id.", nameof(models)),
            _ => throw new ArgumentException("Bulk user permission updates must target a single user.", nameof(models))
        };
    }

    private static string GetSingleRoleId(IReadOnlyCollection<PermissionModel> models)
    {
        var roleIds = models.Select(model => model.RoleId)
            .Where(roleId => !string.IsNullOrWhiteSpace(roleId))
            .Distinct()
            .ToList();

        return roleIds.Count switch
        {
            1 => roleIds[0]!,
            0 => throw new ArgumentException("Permission models must include a role id.", nameof(models)),
            _ => throw new ArgumentException("Bulk role permission updates must target a single role.", nameof(models))
        };
    }

    private void EnsureSucceeded(IdentityResult result, string action, string entityId)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = string.Join(", ", result.Errors.Select(error => error.Description));
        _logger.LogWarning("Failed to {Action} for {EntityId}: {Errors}", action, entityId, errors);
        throw new InvalidOperationException($"Failed to {action} for '{entityId}': {errors}");
    }
}