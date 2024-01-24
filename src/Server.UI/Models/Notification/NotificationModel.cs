namespace CleanArchitecture.Blazor.Server.UI.Models.Notification;

public class NotificationModel
{
    public NotificationTypes NotificationType { get; set; }

    public string NotificationIcon
    {
        get
        {
            return NotificationType switch
            {
                NotificationTypes.NewMessage => Icons.Material.Filled.Message,
                NotificationTypes.NewEmail => Icons.Material.Filled.Email,
                NotificationTypes.CommentLiked => Icons.Material.Filled.ThumbUp,
                NotificationTypes.CommentAnswered => Icons.Material.Filled.Message,
                NotificationTypes.OrderPlaced => Icons.Material.Filled.ShoppingCart,
                _ => string.Empty
            };
        }
    }

    public string Message { get; set; } = string.Empty;
    public DateTime DateTimeStamp { get; set; }
    public bool IsActive { get; set; }

    public string TimeSinceEventFormatted
    {
        get
        {
            var timeSinceEvent = DateTime.Now - DateTimeStamp;

            if (timeSinceEvent.TotalSeconds < 60)
                return $"{timeSinceEvent.Seconds} seconds ago";
            if (timeSinceEvent.TotalMinutes < 60)
                return $"{timeSinceEvent.Minutes} minutes ago";
            if (timeSinceEvent.TotalHours < 24)
                return $"{timeSinceEvent.Hours} hours ago";
            if (timeSinceEvent.TotalDays < 7)
                return $"{timeSinceEvent.Days} days ago";
            if (timeSinceEvent.TotalDays < 30)
                return $"{timeSinceEvent.Days / 7} weeks ago";
            if (timeSinceEvent.TotalDays < 365)
                return $"{timeSinceEvent.Days / 30} months ago";

            return $"{timeSinceEvent.Days / 365} years ago";
        }
    }
}