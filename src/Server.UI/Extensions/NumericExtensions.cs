using System.Globalization;

namespace CleanArchitecture.Blazor.Server.UI.Extensions;

public static class NumericExtensions
{
    public static string RoundedToString(this decimal? num)
    {
        return num switch
        {
            null => "0",
            > 999999999 or < -999999999 => num.Value.ToString("0,,,.#B", CultureInfo.InvariantCulture),
            > 999999 or < -999999 => num.Value.ToString("0,,.#M", CultureInfo.InvariantCulture),
            > 9999 or < -9999 => num.Value.ToString("0,k", CultureInfo.InvariantCulture),
            > 999 or < -999 => num.Value.ToString("0,.#k", CultureInfo.InvariantCulture),
            _ => num.Value.ToString(CultureInfo.InvariantCulture)
        };
    }
}