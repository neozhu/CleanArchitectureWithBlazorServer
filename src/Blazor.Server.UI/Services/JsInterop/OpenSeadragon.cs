using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace Blazor.Server.UI.Services.JsInterop;

[SupportedOSPlatform("browser")]
public partial class OpenSeadragon
{
    [JSImport("", "")]
    internal static partial Task Show(string element, string url);
}
