using CleanArchitecture.Blazor.Server.UI.Services;
using CleanArchitecture.Blazor.Server.UI.Services.Layout;
using Microsoft.AspNetCore.Components.Web;

namespace CleanArchitecture.Blazor.Server.UI.Components.Shared.Themes;

public partial class ThemesButton
{
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    
    [Inject] private LayoutService LayoutService { get; set; } = default!;

}