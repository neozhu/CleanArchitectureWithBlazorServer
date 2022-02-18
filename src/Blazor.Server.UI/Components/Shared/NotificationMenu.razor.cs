using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Utilities;
using Blazor.Server.UI.Models.Notification;
using Blazor.Server.UI.Services;
using Blazor.Server.UI.Services.Notifications;

namespace Blazor.Server.UI.Components.Shared;

public partial class NotificationMenu : MudComponentBase
{

    private bool _newNotificationsAvailable = false;
    private IDictionary<NotificationMessage, bool> _messages = null;
    [Inject] private INotificationService NotificationService { get; set; } = default!;
    private async Task MarkNotificationAsRead()
    {
        await NotificationService.MarkNotificationsAsRead();
        _newNotificationsAvailable = false;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
           (NotificationService as InMemoryNotificationService).Preload();
        }
        _newNotificationsAvailable = await NotificationService.AreNewNotificationsAvailable();
        _messages = await NotificationService.GetNotifications();
        StateHasChanged();
    }
   
}