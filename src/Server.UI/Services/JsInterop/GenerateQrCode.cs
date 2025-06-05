using Microsoft.JSInterop;

namespace CleanArchitecture.Blazor.Server.UI.Services.JsInterop;

public sealed class GenerateQrCode
{
    private readonly IJSRuntime _jsRuntime;

    public GenerateQrCode(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<string> Generate(string defaultUrl)
    {
    
        var jsmodule = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/generateQrCode.js");
        
        // Generate the QR code using the loaded library
        return await jsmodule.InvokeAsync<string>("generateQRCode", defaultUrl);
    }
}