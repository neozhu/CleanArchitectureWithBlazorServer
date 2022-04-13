using System.Globalization;
using System.Resources;

namespace CleanArchitecture.Blazor.Application.Common.Helper;
public static class ConstantStringLocalizer
{
    private static readonly ResourceManager rm;
    static ConstantStringLocalizer() {
        rm = new ResourceManager("CleanArchitecture.Blazor.Application.Resources.Constants.Constants", typeof(ConstantStringLocalizer).Assembly);
    }
    public static string Localize(string key)
    {
        return rm.GetString(key, CultureInfo.CurrentCulture) ?? key;
    }
}
