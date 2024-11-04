namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

/// <summary>
/// Represents the current user context, holding session information.
/// </summary>
public class CurrentUserContext : ICurrentUserContext
{
    /// <summary>
    /// Gets or sets the session information of the current user.
    /// </summary>
    public SessionInfo? SessionInfo { get; set; }
}
