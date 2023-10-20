using CleanArchitecture.Blazor.Server.UI.Models.Identity;
using Microsoft.JSInterop;

namespace CleanArchitecture.Blazor.Server.UI.Services.JsInterop;

public partial class OrgChart
{
    private readonly IJSRuntime _jsRuntime;

    public OrgChart(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    public ValueTask Create(List<OrgItem> data)
    {
        return _jsRuntime.InvokeVoidAsync(JSInteropConstants.CreateOrgChart, data);
    }
}
