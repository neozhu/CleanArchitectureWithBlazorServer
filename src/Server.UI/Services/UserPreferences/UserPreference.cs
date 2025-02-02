// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Color = System.Drawing.Color;

namespace CleanArchitecture.Blazor.Server.UI.Services.UserPreferences;



public class UserPreference
{
    // List of available primary colors
    public static readonly List<string> PrimaryColors = new()
    {
        "#2d4275",
        "#7928ca",
        "#4CAF50",
        "#f7b955",
        "#f33f33",
        "#ff0080"
    };

    // List of available dark primary colors
    public static readonly List<string> DarkPrimaryColors = new()
    {
        "#0170f3",
        "#7928ca",
        "#50e3c2",
        "#f33f33",
        "#ff0080",
        "#666666"
    };
    /// <summary>
    /// Indicates whether the dark mode is enabled.
    /// </summary>
    public bool IsDarkMode { get; set; }
    /// <summary>
    /// Indicates whether the layout is right-to-left.
    /// </summary>
    public bool RightToLeft { get; set; }

    /// <summary>
    /// Scale factor used for typography.
    /// </summary>
    public const double ScaleFactor = 1.067;

    /// <summary>
    /// The primary color for light mode.
    /// </summary>
    public string PrimaryColor { get; set; } = "#2d4275";

    /// <summary>
    /// The primary color for dark mode.
    /// </summary>
    public string DarkPrimaryColor { get; set; } = "#8b9ac6";

    /// <summary>
    /// Returns a darker version of the primary color.
    /// </summary>
    public string PrimaryDarken => AdjustBrightness(PrimaryColor, 0.8);

    /// <summary>
    /// Returns a lighter version of the primary color.
    /// </summary>
    public string PrimaryLighten => AdjustBrightness(PrimaryColor, 0.7);

    /// <summary>
    /// The secondary color.
    /// </summary>
    public string SecondaryColor { get; set; } = "rgba(255, 255, 255, 0.7)";

    /// <summary>
    /// The border radius (in pixels) for UI elements.
    /// </summary>
    public double BorderRadius { get; set; } = 4;

    /// <summary>
    /// The default font size (in rem).
    /// </summary>
    public double DefaultFontSize { get; set; } = 0.8125;

    /// <summary>
    /// Calculates the line height based on the default font size.
    /// </summary>
    public double LineHeight => Math.Min(1.7, Math.Max(1.3, 1.43 * (DefaultFontSize / 0.875)));

    /// <summary>
    /// Calculates the letter spacing based on the default font size.
    /// </summary>
    public double LetterSpacing => 0.00938 * (DefaultFontSize / 0.875);

    // Heading font sizes (calculated using the default font size and the scale factor)
    public double H6FontSize => DefaultFontSize * 1.125 * ScaleFactor;
    public double H5FontSize => DefaultFontSize * 1.25 * ScaleFactor;
    public double H4FontSize => DefaultFontSize * 1.5 * ScaleFactor;
    public double H3FontSize => DefaultFontSize * 1.875 * ScaleFactor;
    public double H2FontSize => DefaultFontSize * 2.25 * ScaleFactor;
    public double H1FontSize => DefaultFontSize * 3 * ScaleFactor;

    // Body text properties
    public double Body1FontSize => DefaultFontSize;
    public double Body1LineHeight => LineHeight;
    public double Body1LetterSpacing => LetterSpacing;
    public double Body2FontSize => DefaultFontSize - 0.0625;
    public double Body2LineHeight => LineHeight;
    public double Body2LetterSpacing => LetterSpacing;

    // Button text properties
    public double ButtonFontSize => DefaultFontSize;
    public double ButtonLineHeight => Math.Min(2.15, Math.Max(1.5, 1.75 * (DefaultFontSize / 0.875)));

    // Caption text properties
    public double CaptionFontSize => DefaultFontSize - 0.1875;
    public double CaptionLineHeight => Math.Min(1.8, Math.Max(1.3, 1.4 * (DefaultFontSize / 0.625)));

    // Overline text properties
    public double OverlineFontSize => DefaultFontSize - 0.1875;
    public double OverlineLineHeight => Math.Min(2.0, Math.Max(1.5, 1.6 * (DefaultFontSize / 0.625)));

    // Subtitle text properties
    public double Subtitle1FontSize => DefaultFontSize + 0.125;
    public double Subtitle2FontSize => DefaultFontSize - 0.0625;

    /// <summary>
    /// The theme mode (System, Light, or Dark).
    /// </summary>
    public DarkLightMode DarkLightTheme { get; set; }

    /// <summary>
    /// Adjusts the brightness of a hex color.
    /// </summary>
    /// <param name="hexColor">The hex color code.</param>
    /// <param name="factor">
    /// The factor by which to adjust brightness (values less than 1 darken the color,
    /// values greater than 1 lighten the color).
    /// </param>
    /// <returns>The adjusted hex color code.</returns>
    private string AdjustBrightness(string hexColor, double factor)
    {
        if (string.IsNullOrWhiteSpace(hexColor))
            throw new ArgumentException("Color code cannot be null or empty.", nameof(hexColor));

        // Parse the hex color using ColorTranslator
        Color color = ColorTranslator.FromHtml(hexColor);

        // Convert RGB to HSL
        ColorToHsl(color, out double h, out double s, out double l);

        // Adjust lightness
        l = Math.Clamp(l * factor, 0.0, 1.0);

        // Convert HSL back to Color
        Color adjustedColor = HslToColor(h, s, l);

        // Return the hex representation
        return ColorTranslator.ToHtml(adjustedColor);
    }

    /// <summary>
    /// Converts a System.Drawing.Color from RGB to HSL.
    /// </summary>
    /// <param name="color">The input color.</param>
    /// <param name="h">Output hue component (0 to 1).</param>
    /// <param name="s">Output saturation component (0 to 1).</param>
    /// <param name="l">Output lightness component (0 to 1).</param>
    private void ColorToHsl(Color color, out double h, out double s, out double l)
    {
        // Normalize RGB values to the 0-1 range.
        double r = color.R / 255.0;
        double g = color.G / 255.0;
        double b = color.B / 255.0;

        double max = Math.Max(r, Math.Max(g, b));
        double min = Math.Min(r, Math.Min(g, b));
        l = (max + min) / 2.0;

        if (max == min)
        {
            // Achromatic color (no hue)
            h = s = 0.0;
        }
        else
        {
            double delta = max - min;
            s = l > 0.5 ? delta / (2.0 - max - min) : delta / (max + min);

            if (max == r)
                h = (g - b) / delta + (g < b ? 6.0 : 0.0);
            else if (max == g)
                h = (b - r) / delta + 2.0;
            else
                h = (r - g) / delta + 4.0;

            h /= 6.0;
        }
    }

    /// <summary>
    /// Converts HSL values to a System.Drawing.Color.
    /// </summary>
    /// <param name="h">The hue component (0 to 1).</param>
    /// <param name="s">The saturation component (0 to 1).</param>
    /// <param name="l">The lightness component (0 to 1).</param>
    /// <returns>The corresponding Color.</returns>
    private Color HslToColor(double h, double s, double l)
    {
        double r, g, b;

        if (s == 0.0)
        {
            // Achromatic color (gray)
            r = g = b = l;
        }
        else
        {
            double HueToRgb(double p, double q, double t)
            {
                if (t < 0.0) t += 1.0;
                if (t > 1.0) t -= 1.0;
                if (t < 1.0 / 6.0) return p + (q - p) * 6.0 * t;
                if (t < 1.0 / 2.0) return q;
                if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6.0;
                return p;
            }

            double q = l < 0.5 ? l * (1.0 + s) : l + s - l * s;
            double p = 2.0 * l - q;

            r = HueToRgb(p, q, h + 1.0 / 3.0);
            g = HueToRgb(p, q, h);
            b = HueToRgb(p, q, h - 1.0 / 3.0);
        }

        // Convert normalized values back to 0-255 and return a Color.
        return Color.FromArgb(
            (int)Math.Round(r * 255.0),
            (int)Math.Round(g * 255.0),
            (int)Math.Round(b * 255.0)
        );
    }
}

public enum DarkLightMode
{
    System = 0,
    Light = 1,
    Dark = 2
}
