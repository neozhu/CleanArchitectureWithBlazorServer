using Blazor.Server.UI.Pages.Identity.Users;
using Microsoft.JSInterop;

namespace Blazor.Server.UI.Services.JsInterop;


public partial class OrgChart
{
    private readonly IJSRuntime _jsRuntime;

    public OrgChart(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    public async Task<ValueTask> Create(List<OrgItem> data)
    {
        var jsmodule = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/orgchart.js");
        return jsmodule.InvokeVoidAsync("createOrgChart", data);
    }


}
