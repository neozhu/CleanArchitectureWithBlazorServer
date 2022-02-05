using MudDemo.Server.Models.Notification;

namespace MudDemo.Server.Services;

public interface INotificationsService
{
    Task<IEnumerable<NotificationModel>> GetNotifications();
    Task<IEnumerable<NotificationModel>> GetActiveNotifications();
}