
using Microsoft.JSInterop;

namespace CleanArchitecture.Blazor.Server.UI.Services.JsInterop;

public sealed class Fancybox
{
    private readonly IJSRuntime _jsRuntime;

    public Fancybox(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    public async Task<ValueTask> Preview(string defaultUrl, IEnumerable<ProductImage> images)
    {
        var jsmodule = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/fancybox.js");
        return jsmodule.InvokeVoidAsync(JSInteropConstants.PreviewImage, defaultUrl, images.Select(x => x.Url).ToArray());
    }
}
