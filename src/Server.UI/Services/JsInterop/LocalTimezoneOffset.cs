using Microsoft.JSInterop;

namespace CleanArchitecture.Blazor.Server.UI.Services.JsInterop;

public class LocalTimezoneOffset
{
    private readonly IJSRuntime _jsRuntime;

    public LocalTimezoneOffset(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async ValueTask<TimeSpan> GetLocalOffset()
    {
        var jsmodule = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/gettimezoneoffset.js").ConfigureAwait(false);
        var hoursOffset = await jsmodule.InvokeAsync<int>(JSInteropConstants.GetTimezoneOffset).ConfigureAwait(false);
        return TimeSpan.FromHours(hoursOffset);
    }

    public async ValueTask<TimeSpan> GetOffsetForTimezone(string timezone)
    {
        var jsmodule = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/gettimezoneoffset.js").ConfigureAwait(false);
        var hoursOffset = await jsmodule.InvokeAsync<int>(JSInteropConstants.GetTimezoneOffsetByTimeZone, timezone).ConfigureAwait(false);
        return TimeSpan.FromHours(hoursOffset);
    }
}