
using Blazor.Server.UI.Models.SideMenu;
using Blazor.Server.UI.Services;
using Blazor.Server.UI.Services.Navigation;


namespace Blazor.Server.UI.Components.Shared;

public partial class SideMenu : IDisposable
{
  
    private UserProfile UserProfile => new UserProfile();

    [EditorRequired] [Parameter] 
    public bool SideMenuDrawerOpen { get; set; } 
    
    [EditorRequired] [Parameter]
    public EventCallback<bool> SideMenuDrawerOpenChanged { get; set; }

    [Inject] 
    private IMenuService _menuService { get; set; } = default!;
    private IEnumerable<MenuSectionModel> _menuSections => _menuService.Features;
    
    [Inject] 
    private LayoutService LayoutService { get; set; } = default!;

    private string[] _roles => UserProfile.AssignRoles??new string[] { };

    public void Dispose()
    {
        
    }
}