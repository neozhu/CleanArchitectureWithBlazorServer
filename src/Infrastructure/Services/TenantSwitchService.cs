using System.Security.Claims;
using CleanArchitecture.Blazor.Application.Common.Constants.ClaimTypes;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Common.Security;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;
using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

/// <summary>
/// Service for managing tenant switching functionality
/// </summary>
public class TenantSwitchService : ITenantSwitchService
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IPermissionService _permissionService;
    private readonly UserProfileStateService _userProfileStateService;
    private readonly ILogger<TenantSwitchService> _logger;

    public TenantSwitchService(
        IApplicationDbContextFactory dbContextFactory,
        IServiceScopeFactory serviceScopeFactory,
        IPermissionService permissionService,
        UserProfileStateService userProfileStateService,
        ILogger<TenantSwitchService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _serviceScopeFactory = serviceScopeFactory;
        _permissionService = permissionService;
        _userProfileStateService = userProfileStateService;
        _logger = logger;
    }

    /// <summary>
    /// Get list of available tenants for the specified user
    /// </summary>
    public async Task<List<TenantDto>> GetAvailableTenantsAsync(string userId)
    {
        try
        {
            await using var db = await _dbContextFactory.CreateAsync();
            
            // Get all tenants that the user can switch to
            var availableTenants = new List<TenantDto>();
            
            // Check if user has permission to switch to any tenant
            var canSwitchToAnyTenant = await _permissionService.HasPermissionAsync(Permissions.Users.SwitchToAnyTenant);
            
            if (canSwitchToAnyTenant)
            {
                // User can switch to any tenant
                var allTenants = await db.Tenants
                    .Select(t => new TenantDto { Id = t.Id, Name = t.Name, Description = t.Description })
                    .ToListAsync();
                    
                availableTenants.AddRange(allTenants);
            }
            return availableTenants.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get available tenants for user {UserId}", userId);
            return new List<TenantDto>();
        }
    }

    /// <summary>
    /// Switch user to specified tenant
    /// </summary>
    public async Task<Result> SwitchToTenantAsync(string userId, string tenantId)
    {
        try
        {
            // Validate permissions
            if (!await CanSwitchToTenantAsync(userId, tenantId))
                return Result.Failure("Insufficient permissions to switch to this tenant");

            await using var db = await _dbContextFactory.CreateAsync();
            
            // Get user and tenant information
            using var scope = _serviceScopeFactory.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            
            var user = await userManager.FindByIdAsync(userId);
            var tenant = await db.Tenants.FindAsync(tenantId);
            
            if (user == null || tenant == null)
                return Result.Failure("User or tenant not found");

            // Get user's current role names
            var currentRoleNames = await userManager.GetRolesAsync(user);
            
            // Find roles with same names in the new tenant
            var newTenantRoles = await roleManager.Roles
                .Where(r => r.TenantId == tenantId)
                .ToListAsync();

            var rolesToAssign = new List<string>();
            var missingRoles = new List<string>();
            var clonedRoles = new List<string>();

            foreach (var currentRoleName in currentRoleNames)
            {
                // Find role with same name in the new tenant
                var matchingRole = newTenantRoles.FirstOrDefault(r => r.Name == currentRoleName);
                
                if (matchingRole != null)
                {
                    // Found matching role, add to assignment list
                    rolesToAssign.Add(currentRoleName);
                }
                else
                {
                    // Role not found in new tenant, clone it from current tenant
                    if (!string.IsNullOrEmpty(user.TenantId))
                    {
                        var clonedRole = await CloneRoleToTenantAsync(currentRoleName, user.TenantId, tenantId, roleManager, db);
                        if (clonedRole != null && !string.IsNullOrEmpty(clonedRole.Name))
                        {
                            rolesToAssign.Add(clonedRole.Name);
                            clonedRoles.Add(clonedRole.Name);
                        }
                        else
                        {
                            // Failed to clone role
                            missingRoles.Add(currentRoleName);
                        }
                    }
                    else
                    {
                        // User has no current tenant, can't clone roles
                        missingRoles.Add(currentRoleName);
                    }
                }
            }

            // If no matching roles found and no roles were cloned, assign default roles
            if (!rolesToAssign.Any())
            {
                _logger.LogError("No matching roles found and no roles could be cloned for user {UserId} to tenant {TenantId}", userId, tenantId);
            }

            // Update user's tenant ID
            user.TenantId = tenantId;
            
            // Remove current roles
            await userManager.RemoveFromRolesAsync(user, currentRoleNames);
            
            // Assign new roles
            if (rolesToAssign.Any())
            {
                await userManager.AddToRolesAsync(user, rolesToAssign);
            }
            
            // Update database
            await userManager.UpdateAsync(user);
            
            // Refresh user state and cache
            await _userProfileStateService.RefreshAsync(user.UserName!);
            
            // Update user claims
            await RefreshUserClaimsAsync(user, userManager);
            
            // Record switch result
            var result = Result.Success();
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to switch user {UserId} to tenant {TenantId}", userId, tenantId);
            return Result.Failure("Failed to switch tenant");
        }
    }

    /// <summary>
    /// Check if user can switch to specified tenant
    /// </summary>
    public async Task<bool> CanSwitchToTenantAsync(string userId, string tenantId)
    {
        try
        {
            // Check if user has permission to switch tenants
            var hasSwitchPermission = await _permissionService.HasPermissionAsync(Permissions.Users.SwitchTenants);
            if (!hasSwitchPermission)
                return false;

            // Check if user has permission to switch to any tenant
            var hasAnyTenantPermission = await _permissionService.HasPermissionAsync(Permissions.Users.SwitchToAnyTenant);
            if (hasAnyTenantPermission)
                return true;

            // Regular users can only switch to tenants where they have roles
            using var scope = _serviceScopeFactory.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return false;
                
            var userRoleNames = await userManager.GetRolesAsync(user);
            var userRoles = await roleManager.Roles
                .Where(r => userRoleNames.Contains(r.Name!) && r.TenantId == tenantId)
                .ToListAsync();

            // Check if user has roles in the target tenant
            return userRoles.Any();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check tenant switch permission for user {UserId} to tenant {TenantId}", userId, tenantId);
            return false;
        }
    }

    /// <summary>
    /// Get role mappings for user when switching to target tenant
    /// </summary>
    public async Task<List<RoleMappingDto>> GetRoleMappingsAsync(string userId, string targetTenantId)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            
            // Get user's current roles
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return new List<RoleMappingDto>();
                
            var currentRoleNames = await userManager.GetRolesAsync(user);
            
            // Get all roles in target tenant
            var targetTenantRoles = await roleManager.Roles
                .Where(r => r.TenantId == targetTenantId)
                .ToListAsync();

            var mappings = new List<RoleMappingDto>();
            
            foreach (var currentRoleName in currentRoleNames)
            {
                var matchingRole = targetTenantRoles.FirstOrDefault(r => r.Name == currentRoleName);
                
                mappings.Add(new RoleMappingDto
                {
                    CurrentRoleName = currentRoleName,
                    TargetRoleName = matchingRole?.Name,
                    WillBeAssigned = matchingRole != null,
                    TargetRoleId = matchingRole?.Id
                });
            }

            return mappings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get role mappings for user {UserId} to tenant {TenantId}", userId, targetTenantId);
            return new List<RoleMappingDto>();
        }
    }

    /// <summary>
    /// Refresh user claims after tenant switch
    /// </summary>
    private async Task RefreshUserClaimsAsync(ApplicationUser user, UserManager<ApplicationUser> userManager)
    {
        try
        {
            // Get existing claims
            var existingClaims = await userManager.GetClaimsAsync(user);
            
            // Remove only tenant-related claims that need to be updated
            var tenantClaimsToRemove = existingClaims
                .Where(c => c.Type == ApplicationClaimTypes.TenantId || c.Type == ApplicationClaimTypes.TenantName)
                .ToList();
            
            if (tenantClaimsToRemove.Any())
            {
                await userManager.RemoveClaimsAsync(user, tenantClaimsToRemove);
            }
            
            // Add updated tenant claims
            var newTenantClaims = new List<Claim>
            {
                new(ApplicationClaimTypes.TenantId, user.TenantId ?? ""),
                new(ApplicationClaimTypes.TenantName, user.Tenant?.Name ?? "")
            };
            
            await userManager.AddClaimsAsync(user, newTenantClaims);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh claims for user {UserId}", user.Id);
        }
    }

    /// <summary>
    /// Clone a role from source tenant to target tenant including all permissions
    /// </summary>
    private async Task<ApplicationRole?> CloneRoleToTenantAsync(string roleName, string sourceTenantId, string targetTenantId, 
        RoleManager<ApplicationRole> roleManager, IApplicationDbContext db)
    {
        try
        {
            // Get the source role with all its claims
            var sourceRole = await roleManager.Roles
                .Include(r => r.RoleClaims)
                .FirstOrDefaultAsync(r => r.Name == roleName && r.TenantId == sourceTenantId);

            if (sourceRole == null)
            {
                return null;
            }

            // Check if role already exists in target tenant
            var existingRole = await roleManager.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName && r.TenantId == targetTenantId);

            if (existingRole != null)
            {
                return existingRole;
            }

            // Create new role in target tenant
            var newRole = new ApplicationRole
            {
                Name = sourceRole.Name,
                TenantId = targetTenantId,
                Description = sourceRole.Description,
                NormalizedName = sourceRole.NormalizedName,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            // Add the new role
            var createResult = await roleManager.CreateAsync(newRole);
            if (!createResult.Succeeded)
            {
                _logger.LogError("Failed to create role {RoleName} in target tenant {TargetTenantId}: {Errors}", 
                    roleName, targetTenantId, string.Join(", ", createResult.Errors.Select(e => e.Description)));
                return null;
            }

            // Clone all role claims (permissions) using RoleManager
            if (sourceRole.RoleClaims != null && sourceRole.RoleClaims.Any())
            {
                foreach (var claim in sourceRole.RoleClaims)
                {
                    if (!string.IsNullOrEmpty(claim.ClaimType) && !string.IsNullOrEmpty(claim.ClaimValue))
                    {
                        var addClaimResult = await roleManager.AddClaimAsync(newRole, new Claim(claim.ClaimType, claim.ClaimValue));
                        if (!addClaimResult.Succeeded)
                        {
                            _logger.LogError("Failed to add claim {ClaimType}:{ClaimValue} to role {RoleName}: {Errors}", 
                                claim.ClaimType, claim.ClaimValue, roleName, string.Join(", ", addClaimResult.Errors.Select(e => e.Description)));
                        }
                    }
                }
            }

            return newRole;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clone role {RoleName} from tenant {SourceTenantId} to tenant {TargetTenantId}", 
                roleName, sourceTenantId, targetTenantId);
            return null;
        }
    }
}