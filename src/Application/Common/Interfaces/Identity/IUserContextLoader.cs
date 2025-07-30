using System.Security.Claims;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

/// <summary>
/// Interface for loading user context from ClaimsPrincipal.
/// </summary>
public interface IUserContextLoader
{
    /// <summary>
    /// Loads user context from the provided ClaimsPrincipal.
    /// </summary>
    /// <param name="principal">The ClaimsPrincipal containing user information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The loaded UserContext, or null if the user is not authenticated.</returns>
    Task<UserContext?> LoadAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads user context from the provided ClaimsPrincipal and sets it in the accessor.
    /// </summary>
    /// <param name="principal">The ClaimsPrincipal containing user information.</param>
    /// <param name="accessor">The user context accessor to set the context in.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The loaded UserContext, or null if the user is not authenticated.</returns>
    Task<UserContext?> LoadAndSetAsync(ClaimsPrincipal principal, IUserContextAccessor accessor, CancellationToken cancellationToken = default);
} 