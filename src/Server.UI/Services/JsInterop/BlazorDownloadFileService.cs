using Microsoft.JSInterop;

namespace CleanArchitecture.Blazor.Server.UI.Services.JsInterop;

public class BlazorDownloadFileService
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _module;

    public BlazorDownloadFileService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Downloads a file by invoking a JavaScript function.
    /// </summary>
    /// <param name="fileName">The file name including extension.</param>
    /// <param name="data">The file data as a byte array.</param>
    /// <param name="contentType">The MIME type of the file.</param>
    public async Task DownloadFileAsync(string fileName, byte[] data, string contentType)
    {
        // Load the JavaScript module from wwwroot/js/downloadFile.js if not already loaded
        if (_module == null)
        {
            _module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/downloadFile.js");
        }

        // Convert the byte array to a Base64 string
        string base64Data = Convert.ToBase64String(data);

        // Call the JavaScript function to trigger the file download
        await _module.InvokeVoidAsync("downloadFile", fileName, base64Data, contentType);
    }
}
