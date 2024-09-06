using Microsoft.JSInterop;

namespace CleanArchitecture.Blazor.Server.UI.Services.JsInterop;

public class HistoryGo
{
    private readonly IJSRuntime _jsRuntime;

    public HistoryGo(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<ValueTask> GoBack(int  value=-1)
    {
        var jsmodule = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/historygo.js");
        return jsmodule.InvokeVoidAsync(JSInteropConstants.HistoryGo, value);
    }
}