using CleanArchitecture.Blazor.Application.Common.Constants;
using CleanArchitecture.Blazor.Application.Common.Extensions;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

/// <summary>
/// Implementation of IUserContextLoader that uses UserManager to load user context from ClaimsPrincipal.
/// </summary>
public class UserContextLoader : IUserContextLoader
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IFusionCache _fusionCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserContextLoader"/> class.
    /// </summary>
    /// <param name="scopeFactory">The service scope factory.</param>
    /// <param name="fusionCache">The fusion cache instance.</param>
    public UserContextLoader(IServiceScopeFactory scopeFactory, IFusionCache fusionCache)
    {
        _scopeFactory = scopeFactory;
        _fusionCache = fusionCache;
    }

    /// <summary>
    /// Loads user context from the provided ClaimsPrincipal.
    /// </summary>
    /// <param name="principal">The ClaimsPrincipal containing user information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The loaded UserContext, or null if the user is not authenticated.</returns>
    public async Task<UserContext?> LoadAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default)
    {
        if (principal?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var userId = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        var cacheKey = UserCacheKeys.GetCacheKey(userId, UserCacheType.Context);

        return await _fusionCache.GetOrSetAsync(
            cacheKey,
            async _ =>
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var user = await userManager.GetUserAsync(principal);
                    if (user == null)
                    {
                        return null;
                    }

                    var roles = await userManager.GetRolesAsync(user);

                    return new UserContext(
                        UserId: user.Id,
                        UserName: user.UserName ?? string.Empty,
                        DisplayName: user.DisplayName,
                        TenantId: user.TenantId,
                        Email: user.Email,
                        Roles: roles.ToList().AsReadOnly(),
                        ProfilePictureDataUrl: user.ProfilePictureDataUrl,
                        SuperiorId: user.SuperiorId
                    );
                }
                catch (Exception)
                {
                    return null;
                }
            },
            options:new FusionCacheEntryOptions(TimeSpan.FromHours(1)),
            cancellationToken
        );
    }

    /// <summary>
    /// Clears the cached user context for a specific user.
    /// </summary>
    /// <param name="userId">The user ID to clear cache for.</param>
    public void ClearUserContextCache(string userId)
    {
        if (!string.IsNullOrEmpty(userId))
        {
            var cacheKey = UserCacheKeys.GetCacheKey(userId, UserCacheType.Context);
            _fusionCache.Remove(cacheKey);
        }
    }

    
} 
