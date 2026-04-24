using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Blazor.Server.UI.Models.Notification;

public enum NotificationTypes
{
    [Display(Name = "New Message")]
    NewMessage,

    [Display(Name = "New Email")]
    NewEmail,

    [Display(Name = "Comment Liked")]
    CommentLiked,

    [Display(Name = "Comment Answered")]
    CommentAnswered,

    [Display(Name = "Order Placed")]
    OrderPlaced,

    [Display(Name = "Order Received")]
    OrderReceived
}
