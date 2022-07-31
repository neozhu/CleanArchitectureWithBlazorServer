using $safeprojectname$.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace $safeprojectname$.Components.Shared.Themes;

public partial class ThemesButton
{
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    
    [Inject] private LayoutService LayoutService { get; set; } = default!;

}