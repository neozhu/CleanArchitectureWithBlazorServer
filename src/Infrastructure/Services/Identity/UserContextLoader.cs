using CleanArchitecture.Blazor.Application.Common.Extensions;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

/// <summary>
/// Implementation of IUserContextLoader that uses UserManager to load user context from ClaimsPrincipal.
/// </summary>
public class UserContextLoader : IUserContextLoader
{
    private readonly IServiceScopeFactory _scopeFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserContextLoader"/> class.
    /// </summary>
    /// <param name="scopeFactory">The service scope factory.</param>
    public UserContextLoader(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
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
        var context = await LoadAsync(principal, cancellationToken);
        if (context != null)
        {
            accessor.Set(context);
        }
        else
        {
            accessor.Clear();
        }
        return context;
    }
} 