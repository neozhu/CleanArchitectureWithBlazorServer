using Microsoft.JSInterop;

namespace CleanArchitecture.Blazor.Server.UI.Services.JsInterop;

public class InputClear
{
    private readonly IJSRuntime _jsRuntime;

    public InputClear(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<ValueTask> Clear(string targetId)
    {
        var jsmodule = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/clearinput.js");
        return jsmodule.InvokeVoidAsync(JSInteropConstants.ClearInput, targetId);
    }
}