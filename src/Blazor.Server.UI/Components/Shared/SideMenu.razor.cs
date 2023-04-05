using Blazor.Server.UI.Models.SideMenu;
using Blazor.Server.UI.Services;
using Blazor.Server.UI.Services.Navigation;
using CleanArchitecture.Blazor.Application.Features.Identity.Notification;

namespace Blazor.Server.UI.Components.Shared;

public partial class SideMenu:INotificationHandler<UpdateUserProfileCommand>,IDisposable
{

    private UserProfile? UserProfile { get; set; } = null!;

    [EditorRequired] [Parameter] 
    public bool SideMenuDrawerOpen { get; set; } 
    
    [EditorRequired] [Parameter]
    public EventCallback<bool> SideMenuDrawerOpenChanged { get; set; }

    [Inject] 
    private IMenuService _menuService { get; set; } = default!;
    private IEnumerable<MenuSectionModel> _menuSections => _menuService.Features;
    
    [Inject] 
    private LayoutService LayoutService { get; set; } = default!;

    private string[] _roles => UserProfile?.AssignedRoles??new string[] { };

    protected override void OnInitialized()
    {
        UserProfileChanged += userProfileChangedHandler;
    }

    private void userProfileChangedHandler(object? sender, UpdateUserProfileEventArgs e)
    {
        UserProfile = e.UserProfile;
        InvokeAsync(() => StateHasChanged());
    }
    public void Dispose()
    {
        UserProfileChanged -= userProfileChangedHandler;
    }

    public static event EventHandler<UpdateUserProfileEventArgs> UserProfileChanged=null!;
    public Task Handle(UpdateUserProfileCommand notification, CancellationToken cancellationToken)
    {
        UserProfileChanged?.Invoke(this, new UpdateUserProfileEventArgs() { UserProfile = notification.UserProfile });
        return Task.CompletedTask;
    }
}