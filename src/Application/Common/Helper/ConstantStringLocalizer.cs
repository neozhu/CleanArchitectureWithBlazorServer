using System.Globalization;
using System.Resources;

namespace CleanArchitecture.Blazor.Application.Common.Helper;
public static class ConstantStringLocalizer
{
    public const string RESOURCEID = "CleanArchitecture.Blazor.Application.Common.Resources.ConstantString";
    private static readonly ResourceManager rm;
    static ConstantStringLocalizer() {
        rm = new ResourceManager(RESOURCEID, typeof(ConstantStringLocalizer).Assembly);
    }
    public static string Localize(string key)
    {
        return rm.GetString(key, CultureInfo.CurrentCulture) ?? key;
    }
}
