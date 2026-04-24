using System.Security.Claims;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

/// <summary>
/// Loads cached user context information for an authenticated principal.
/// </summary>
public interface IUserContextLoader
{
    Task<UserContext?> LoadAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default);

    void ClearUserContextCache(string userId);
}
