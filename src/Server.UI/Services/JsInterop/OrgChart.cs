using Microsoft.JSInterop;

namespace CleanArchitecture.Blazor.Server.UI.Services.JsInterop;

public class OrgChart
{
    private readonly IJSRuntime _jsRuntime;

    public OrgChart(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<ValueTask> Create(IList<OrgItem> data)
    {
        var jsmodule = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/orgchart.js").ConfigureAwait(false);
        return jsmodule.InvokeVoidAsync("createOrgChart", data);
    }
}
