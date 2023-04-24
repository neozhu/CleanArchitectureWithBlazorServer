namespace CleanArchitecture.Blazor.Application.Features.Identity.Notification;
public class UpdateUserProfileCommand:INotification
{
    public UserProfile UserProfile { get; set; } = null!;
}
public class UpdateUserProfileEventArgs : EventArgs
{
    public UserProfile UserProfile { get; set; } = null!;
}
