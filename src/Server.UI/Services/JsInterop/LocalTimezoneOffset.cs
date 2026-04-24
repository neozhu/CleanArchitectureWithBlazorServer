using Microsoft.JSInterop;

namespace CleanArchitecture.Blazor.Server.UI.Services.JsInterop;

public class LocalTimeOffset
{
    private readonly IJSRuntime _jsRuntime;

    public LocalTimeOffset(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async ValueTask<TimeSpan> GetLocalOffset()
    {
        var jsmodule = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/gettimezoneoffset.js").ConfigureAwait(false);
        var minutesOffset = await jsmodule.InvokeAsync<int>(JSInteropConstants.GetTimezoneOffset).ConfigureAwait(false);
        return TimeSpan.FromMinutes(minutesOffset);
    }

    public async ValueTask<TimeSpan> GetOffsetForTimezone(string timezone)
    {
        var jsmodule = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/gettimezoneoffset.js").ConfigureAwait(false);
        var minutesOffset = await jsmodule.InvokeAsync<int>(JSInteropConstants.GetTimezoneOffsetByTimeZone, timezone).ConfigureAwait(false);
        return TimeSpan.FromMinutes(minutesOffset);
    }
}
