using System.Security.Claims;
using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using CleanArchitecture.Blazor.Application.Common.Security;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Infrastructure.Services.Identity;
using Microsoft.AspNetCore.Identity;
using ZiggyCreatures.Caching.Fusion;
using CleanArchitecture.Blazor.Application.Common.Constants;

namespace CleanArchitecture.Blazor.Server.UI.Services.Identity;

/// <inheritdoc />
public class UserPermissionAssignmentService : IPermissionAssignmentService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPermissionHelper _permissionHelper;
    private readonly IFusionCache _fusionCache;
    private readonly ILogger<UserPermissionAssignmentService> _logger;

    public UserPermissionAssignmentService(UserManager<ApplicationUser> userManager,
        IPermissionHelper permissionHelper,
        IFusionCache fusionCache,
        ILogger<UserPermissionAssignmentService> logger)
    {
        _userManager = userManager;
        _permissionHelper = permissionHelper;
        _fusionCache = fusionCache;
        _logger = logger;
    }

    public async Task<IList<PermissionModel>> LoadAsync(string entityId)
    {
        return await _permissionHelper.GetAllPermissionsByUserId(entityId);
    }

    public async Task AssignAsync(PermissionModel model)
    {
        var userId = model.UserId ?? throw new ArgumentNullException(nameof(model.UserId));
        var user = await _userManager.FindByIdAsync(userId) ??
                   throw new NotFoundException($"User not found: {userId}");

        var claim = new Claim(model.ClaimType, model.ClaimValue);
        if (model.Assigned)
            await _userManager.AddClaimAsync(user, claim);
        else
            await _userManager.RemoveClaimAsync(user, claim);

        InvalidateCache(userId);
    }

    public async Task AssignBulkAsync(IEnumerable<PermissionModel> models)
    {
        var list = models.ToList();
        if (!list.Any()) return;

        var userId = list.First().UserId ?? string.Empty;
        var user = await _userManager.FindByIdAsync(userId) ??
                   throw new NotFoundException($"User not found: {userId}");

        foreach (var model in list)
        {
            var claim = new Claim(model.ClaimType, model.ClaimValue);
            if (model.Assigned)
                await _userManager.AddClaimAsync(user, claim);
            else
                await _userManager.RemoveClaimAsync(user, claim);
        }

        InvalidateCache(userId);
    }

    private void InvalidateCache(string userId)
    {
        // Clear both claims/permissions and context cache
        _ = Task.Run(async () => 
        {
            var cacheTypes = new[] { UserCacheType.Claims, UserCacheType.Permissions, UserCacheType.Context };
            var tasks = cacheTypes.Select(cacheType =>
            {
                var cacheKey = UserCacheKeys.GetCacheKey(userId, cacheType);
                return _fusionCache.RemoveAsync(cacheKey).AsTask();
            });
            await Task.WhenAll(tasks);
        });
    }
} 
