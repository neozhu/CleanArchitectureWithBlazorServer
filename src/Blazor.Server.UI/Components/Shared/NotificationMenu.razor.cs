using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Utilities;
using Blazor.Server.UI.Models.Notification;
using Blazor.Server.UI.Services;

namespace Blazor.Server.UI.Components.Shared;

public partial class NotificationMenu : MudComponentBase
{
    private string Classname =>
        new CssBuilder()
            .AddClass(Class)
            .Build();

    [Parameter] public IEnumerable<NotificationModel>? Notifications { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> OnClickViewAll { get; set; }

    private bool _newNotificationsAvailable => Notifications != null && Notifications.Any();

    private void MarkNotificationAsRead() => Notifications = null;
}