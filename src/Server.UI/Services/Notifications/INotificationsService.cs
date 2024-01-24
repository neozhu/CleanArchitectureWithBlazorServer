namespace CleanArchitecture.Blazor.Server.UI.Services.Notifications;

public interface INotificationService
{
    Task<bool> AreNewNotificationsAvailable();
    Task MarkNotificationsAsRead();
    Task MarkNotificationsAsRead(string id);
    Task<NotificationMessage> GetMessageById(string id);
    Task<IDictionary<NotificationMessage, bool>> GetNotifications();
    Task AddNotification(NotificationMessage message);
}