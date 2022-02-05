using Blazor.Server.UI.Models.Notification;

namespace Blazor.Server.UI.Services;

public interface INotificationsService
{
    Task<IEnumerable<NotificationModel>> GetNotifications();
    Task<IEnumerable<NotificationModel>> GetActiveNotifications();
}