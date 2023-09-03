using Blazor.Server.UI.Models.NavigationMenu;
using Blazor.Server.UI.Services.Layout;
using Blazor.Server.UI.Services.Navigation;
using CleanArchitecture.Blazor.Application.Features.Fluxor;

namespace Blazor.Server.UI.Components.Shared;

public partial class NavigationMenu : FluxorComponent
{
    [Inject] private IMenuService MenuService { get; set; } = null!;
    [Inject] private IState<UserProfileState> UserProfileState { get; set; } = null!;

    [Inject] private LayoutService LayoutService { get; set; } = default!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = null!;

    [EditorRequired] [Parameter] public EventCallback<bool> DrawerOpenChanged { get; set; }
    [EditorRequired] [Parameter] public bool DrawerOpen { get; set; }

    private UserProfile UserProfile => UserProfileState.Value.UserProfile;
    private bool IsLoading => UserProfileState.Value.IsLoading;
    private IEnumerable<MenuSectionModel> MenuSections => MenuService.Features;
    private string[] Roles => UserProfile.AssignedRoles ?? Array.Empty<string>();

    private bool Expanded(MenuSectionItemModel menu)
    {
        string href = NavigationManager.Uri[(NavigationManager.BaseUri.Length - 1)..];
        return menu is { IsParent: true, MenuItems: not null } &&
               menu.MenuItems.Any(x => !string.IsNullOrEmpty(x.Href) && x.Href.Equals(href));
    }
}