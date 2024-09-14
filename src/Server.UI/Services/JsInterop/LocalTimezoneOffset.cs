using Microsoft.JSInterop;

namespace CleanArchitecture.Blazor.Server.UI.Services.JsInterop;

public class LocalTimezoneOffset
{
    private readonly IJSRuntime _jsRuntime;

    public LocalTimezoneOffset(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async ValueTask<int> HourOffset()
    {
        var jsmodule = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/timezoneoffset.js");
        return await jsmodule.InvokeAsync<int>(JSInteropConstants.GetTimezoneOffset);
    }
}