using Microsoft.JSInterop;

namespace Blazor.Server.UI.Services.JsInterop;

public partial class OpenSeadragon
{
    private readonly IJSRuntime _jsRuntime;

    public OpenSeadragon(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    public ValueTask Open(string url)
    {
        var target = "openseadragon";
        return _jsRuntime.InvokeVoidAsync(JSInteropConstants.ShowOpenSeadragon, target, url);
    }


}
