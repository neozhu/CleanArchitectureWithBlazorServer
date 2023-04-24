using Blazor.Server.UI.Services.Notifications;

namespace Blazor.Server.UI.Components.Shared;

public partial class NotificationMenu : MudComponentBase
{

    private bool _newNotificationsAvailable = false;
    private IDictionary<NotificationMessage, bool>? _messages = null;
    [Inject] public INotificationService NotificationService { get; set; } = null!;
    private async Task MarkNotificationAsRead()
    {
        await NotificationService.MarkNotificationsAsRead();
        _newNotificationsAvailable = false;
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender == true)
        {
            _newNotificationsAvailable = await NotificationService.AreNewNotificationsAvailable();
            _messages = await NotificationService.GetNotifications();
            StateHasChanged();
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}