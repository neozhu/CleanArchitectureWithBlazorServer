using MudBlazor;
using MudBlazor.Utilities;

namespace Blazor.Server.UI.Components.Index;

public partial class MudBlazorCard : MudComponentBase
{
    private string Classname =>
        new CssBuilder()
            .AddClass(Class)
            .Build();
}