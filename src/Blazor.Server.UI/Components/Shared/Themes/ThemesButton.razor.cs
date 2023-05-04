using Blazor.Server.UI.Services;
using Blazor.Server.UI.Services.Layout;
using Microsoft.AspNetCore.Components.Web;

namespace Blazor.Server.UI.Components.Shared.Themes;

public partial class ThemesButton
{
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    
    [Inject] private LayoutService LayoutService { get; set; } = default!;

}