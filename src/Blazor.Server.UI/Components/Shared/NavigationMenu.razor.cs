using Blazor.Server.UI.Models.NavigationMenu;
using Blazor.Server.UI.Services.Layout;
using Blazor.Server.UI.Services.Navigation;
using CleanArchitecture.Blazor.Application.Features.Fluxor;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace Blazor.Server.UI.Components.Shared;

public partial class NavigationMenu : FluxorComponent
{
    [EditorRequired] [Parameter] public bool DrawerOpen { get; set; }
    [Inject] private IMenuService MenuService { get; set; } = null!;
    [Inject]
    private IState<UserProfileState> UserProfileState { get; set; } = null!;
    private UserProfile UserProfile => UserProfileState.Value.UserProfile;
    private bool IsLoading   => UserProfileState.Value.IsLoading;
    
    [EditorRequired] [Parameter]
    public EventCallback<bool> DrawerOpenChanged { get; set; }

    private IEnumerable<MenuSectionModel> MenuSections => MenuService.Features;

    [Inject] private LayoutService LayoutService { get; set; } = default!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = null!;
    private string[] Roles => UserProfile?.AssignedRoles ?? new string[] { };

    private bool Expanded(MenuSectionItemModel menu)
    {
        string href = NavigationManager.Uri.Substring(NavigationManager.BaseUri.Length-1);
        if(menu.IsParent ==true && menu.MenuItems is not null && menu.MenuItems.Any(x => !string.IsNullOrEmpty(x.Href) && x.Href.Equals(href)))
        {
            return true;
        }
        return false;
    }

}