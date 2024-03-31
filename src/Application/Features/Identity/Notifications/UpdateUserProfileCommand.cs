using CleanArchitecture.Blazor.Application.Common.Security;

namespace CleanArchitecture.Blazor.Application.Features.Identity.Notifications;

public class UpdateUserProfileCommand : INotification
{
    public UserProfile UserProfile { get; set; } = null!;
}

public class UpdateUserProfileEventArgs : EventArgs
{
    public UserProfile UserProfile { get; set; } = null!;
}