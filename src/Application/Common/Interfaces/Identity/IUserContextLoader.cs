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


} 