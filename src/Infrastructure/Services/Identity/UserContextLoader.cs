using CleanArchitecture.Blazor.Application.Common.Extensions;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

/// <summary>
/// Implementation of IUserContextLoader that uses UserManager to load user context from ClaimsPrincipal.
/// </summary>
public class UserContextLoader : IUserContextLoader
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UserContextLoader> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserContextLoader"/> class.
    /// </summary>
    /// <param name="scopeFactory">The service scope factory.</param>
    /// <param name="logger">The logger.</param>
    public UserContextLoader(IServiceScopeFactory scopeFactory, ILogger<UserContextLoader> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <summary>
    /// Loads user context from the provided ClaimsPrincipal.
    /// </summary>
    /// <param name="principal">The ClaimsPrincipal containing user information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The loaded UserContext, or null if the user is not authenticated.</returns>
    public async Task<UserContext?> LoadAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("UserContextLoader: LoadAsync called for user {UserName}", principal?.Identity?.Name ?? "anonymous");
        
        if (principal?.Identity?.IsAuthenticated != true)
        {
            _logger.LogInformation("UserContextLoader: User is not authenticated");
            return null;
        }

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.GetUserAsync(principal);
            if (user == null)
            {
                _logger.LogWarning("UserContextLoader: User not found in database");
                return null;
            }

            var roles = await userManager.GetRolesAsync(user);
            _logger.LogInformation("UserContextLoader: Loaded user {UserName} with {RoleCount} roles", user.UserName, roles.Count);

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "UserContextLoader: Error loading user context");
            return null;
        }
    }

    /// <summary>
    /// Loads user context from the provided ClaimsPrincipal and sets it in the accessor.
    /// </summary>
    /// <param name="principal">The ClaimsPrincipal containing user information.</param>
    /// <param name="accessor">The user context accessor to set the context in.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The loaded UserContext, or null if the user is not authenticated.</returns>
    public async Task<UserContext?> LoadAndSetAsync(ClaimsPrincipal principal, IUserContextAccessor accessor, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("UserContextLoader: LoadAndSetAsync called");
        var context = await LoadAsync(principal, cancellationToken);
        if (context != null)
        {
            accessor.Set(context);
            _logger.LogInformation("UserContextLoader: Set user context for {UserName}", context.UserName);
        }
        else
        {
            accessor.Clear();
            _logger.LogInformation("UserContextLoader: Cleared user context");
        }
        return context;
    }
} 