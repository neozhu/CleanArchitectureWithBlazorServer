using Microsoft.AspNetCore.Components.Web;

namespace Blazor.Server.UI.Components.Shared;

public partial class HeaderMenu
{
    [EditorRequired] [Parameter] public bool IsDarkMode { get; set; }
    [EditorRequired] [Parameter] public bool NavigationMenuDrawerOpen { get; set; }
    [EditorRequired] [Parameter] public EventCallback ToggleNavigationMenuDrawer { get; set; }
    [EditorRequired] [Parameter] public EventCallback OpenCommandPalette { get; set; }
    [EditorRequired] [Parameter] public bool RightToLeft { get; set; }
    [EditorRequired] [Parameter] public EventCallback RightToLeftToggle { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> OnSettingClick { get; set; }

}