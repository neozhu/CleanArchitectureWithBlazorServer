using CleanArchitecture.Blazor.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

/// <summary>
/// Provides access to the current user's session information.
/// </summary>
public class CurrentUserAccessor : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICurrentUserContext _currentUserContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUserAccessor"/> class.
    /// </summary>
    /// <param name="currentUserContext">The current user context.</param>
    public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor, ICurrentUserContext currentUserContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _currentUserContext = currentUserContext;
    }

    /// <summary>
    /// Gets the session information of the current user.
    /// </summary>
    public SessionInfo? SessionInfo => _currentUserContext.SessionInfo;

    public string? UserId => (_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated??false)? _httpContextAccessor.HttpContext?.User.GetUserId(): null;

    public string? TenantId => (_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false) ? _httpContextAccessor.HttpContext?.User.GetTenantId() : null;
}

