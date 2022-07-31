using System.Globalization;
using System.Resources;

namespace CleanArchitecture.Blazor.$safeprojectname$.Common.Helper;
public static class ConstantStringLocalizer
{
    public const string CONSTANTSTRINGRESOURCEID = "CleanArchitecture.Blazor.$safeprojectname$.Resources.Constants.ConstantString";
    private static readonly ResourceManager rm;
    static ConstantStringLocalizer() {
        rm = new ResourceManager(CONSTANTSTRINGRESOURCEID, typeof(ConstantStringLocalizer).Assembly);
    }
    public static string Localize(string key)
    {
        return rm.GetString(key, CultureInfo.CurrentCulture) ?? key;
    }
}
