namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

/// <summary>
/// Interface representing the current user context.
/// </summary>
public interface ICurrentUserContext
{
    /// <summary>
    /// Gets or sets the session information of the current user.
    /// </summary>
    SessionInfo? SessionInfo { get; set; }
}
