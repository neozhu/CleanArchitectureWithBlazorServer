// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CleanArchitecture.Blazor.Server.UI.Services.UserPreferences;

public class UserPreferences
{
    /// <summary>
    /// Set the direction layout of the docs to RTL or LTR. If true RTL is used
    /// </summary>
    public bool RightToLeft { get; set; }

    /// <summary>
    /// If true DarkTheme is used. LightTheme otherwise
    /// </summary>
    public bool IsDarkMode { get; set; }
    public string PrimaryColor { get; set; } = "#2d4275";
    public string PrimaryDarken => AdjustBrightness(PrimaryColor,0.8);
    public string PrimaryLighten => AdjustBrightness(PrimaryColor, 0.7);
    public string SecondaryColor { get; set; } = "#ff4081ff";
    public double BorderRadius { get; set; } = 4;
    public double DefaultFontSize { get; set; } = 0.8125;

    public double Body1FontSize => DefaultFontSize;
    public double Body2FontSize => DefaultFontSize - 0.125;
    public double ButtonFontSize => DefaultFontSize;
    public double CaptionFontSize => DefaultFontSize - 0.125;
    public double OverlineFontSize=> DefaultFontSize - 0.125;
    public double Subtitle1FontSize => DefaultFontSize + 0.125;
    public double Subtitle2FontSize => DefaultFontSize;
    public DarkLightMode DarkLightTheme { get; set; }


    private string AdjustBrightness(string hexColor, double factor)
    {
        if (hexColor.StartsWith("#"))
        {
            hexColor = hexColor.Substring(1); // 删除#前缀，如果存在
        }

        if (hexColor.Length != 6)
        {
            throw new ArgumentException("Invalid hex color code. It must be 6 characters long.");
        }

        int r = int.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        int g = int.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        int b = int.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        int newR = (int)Math.Clamp(r * factor, 0, 255);
        int newG = (int)Math.Clamp(g * factor, 0, 255);
        int newB = (int)Math.Clamp(b * factor, 0, 255);

        return $"#{newR:X2}{newG:X2}{newB:X2}";
    }
}
public enum DarkLightMode
{
    System = 0,
    Light = 1,
    Dark = 2
}
