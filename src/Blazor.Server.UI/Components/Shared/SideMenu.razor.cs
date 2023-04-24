using Blazor.Server.UI.Models.SideMenu;
using Blazor.Server.UI.Services;
using Blazor.Server.UI.Services.Navigation;

namespace Blazor.Server.UI.Components.Shared;

public partial class SideMenu : UserProfileStateComponent
{
    [EditorRequired] [Parameter] public bool SideMenuDrawerOpen { get; set; }

    [EditorRequired] [Parameter] public EventCallback<bool> SideMenuDrawerOpenChanged { get; set; }

    [Inject] private IMenuService MenuService { get; set; } = default!;

    private IEnumerable<MenuSectionModel> MenuSections => MenuService.Features;

    [Inject] private LayoutService LayoutService { get; set; } = default!;

    private string[] Roles => UserProfile?.AssignedRoles ?? new string[] { };
}