using Blazor.Server.UI.Models.SideMenu;
using Blazor.Server.UI.Services;
using Blazor.Server.UI.Services.Layout;
using Blazor.Server.UI.Services.Navigation;
using CleanArchitecture.Blazor.Application.Features.Fluxor;
using Fluxor;

namespace Blazor.Server.UI.Components.Shared;

public partial class SideMenu: FluxorComponent
{
    [EditorRequired] [Parameter] public bool SideMenuDrawerOpen { get; set; }
    [Inject] private IMenuService MenuService { get; set; } = null!;
    [Inject]
    private IState<UserProfileState> UserProfileState { get; set; } = null!;
    private UserProfile UserProfile => UserProfileState.Value.UserProfile;
    private bool IsLoading   => UserProfileState.Value.IsLoading;
    
    [EditorRequired] [Parameter]
    public EventCallback<bool> SideMenuDrawerOpenChanged { get; set; }

    private IEnumerable<MenuSectionModel> MenuSections => MenuService.Features;

    [Inject] private LayoutService LayoutService { get; set; } = default!;

    private string[] Roles => UserProfile?.AssignedRoles ?? new string[] { };
}