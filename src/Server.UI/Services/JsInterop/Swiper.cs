using Microsoft.JSInterop;

namespace CleanArchitecture.Blazor.Server.UI.Services.JsInterop;

public sealed class Swiper
{
    private readonly IJSRuntime _jsRuntime;

    public Swiper(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<ValueTask> Initialize(string elment,bool reverse=false)
    {
        var jsmodule = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/carousel.js");
        return jsmodule.InvokeVoidAsync("initializeSwiper", elment, reverse);
    }
}
