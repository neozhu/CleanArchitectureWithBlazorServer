using CleanArchitecture.Blazor.Domain.Identity;
using ZiggyCreatures.Caching.Fusion;
using CleanArchitecture.Blazor.Application.Common.Constants;
using CleanArchitecture.Blazor.Application.Common.Security;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

/// <summary>
/// Service for managing tenant switching functionality
/// </summary>
public class TenantSwitchService : ITenantSwitchService
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IPermissionService _permissionService;
    private readonly IUserProfileState _userProfileState;
    private readonly IUserContextLoader _userContextLoader;
    private readonly ILogger<TenantSwitchService> _logger;

    public TenantSwitchService(
        IApplicationDbContextFactory dbContextFactory,
        IServiceScopeFactory serviceScopeFactory,
        IPermissionService permissionService,
        IUserProfileState userProfileState,
        IUserContextLoader userContextLoader,
        ILogger<TenantSwitchService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _serviceScopeFactory = serviceScopeFactory;
        _permissionService = permissionService;
        _userProfileState = userProfileState;
        _userContextLoader = userContextLoader;
        _logger = logger;
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

            // Record the original tenant ID for logging
            var originalTenantId = user.TenantId;

            

            // Update user's tenant ID
            user.TenantId = tenantId;

           

            // Update database
            await userManager.UpdateAsync(user);
            
            // Refresh user state and cache
            await _userProfileState.RefreshAsync();

            // Clear user context cache
            _userContextLoader.ClearUserContextCache(userId);
            
            // Update user claims
            await RefreshUserClaimsAsync(user, userManager);
            
            // Log successful tenant switch
            _logger.LogInformation("User {UserId} ({UserName}) successfully switched from tenant {OriginalTenantId} to tenant {NewTenantId} ({TenantName})", 
                userId, user.UserName, originalTenantId ?? "null", tenantId, tenant.Name);
            
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
            if (!hasAnyTenantPermission)
                return false;

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check tenant switch permission for user {UserId} to tenant {TenantId}", userId, tenantId);
            return false;
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

     
}
