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
        "#172554",
        "#7f22fe",
        "#5ea500",
        "#fcc800",
        "#e7000b",
        "#171717"
    };

    // List of available dark primary colors
    public static readonly List<string> DarkPrimaryColors = new()
    {
        "#2b7fff",
        "#8e51ff",
        "#5ea500",
        "#ff6900",
        "#ff2056",
        "#e5e5e5"
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


    /// <summary>
    /// The primary color for light mode.
    /// </summary>
    public string PrimaryColor { get; set; } = PrimaryColors[0];

    /// <summary>
    /// The primary color for dark mode.
    /// </summary>
    public string DarkPrimaryColor { get; set; } = DarkPrimaryColors[0];

    /// <summary>
    /// Returns a darker version of the primary color for hover states.
    /// </summary>
    public string PrimaryDarken => AdjustBrightnessForHover(PrimaryColor, true);

    /// <summary>
    /// Returns a lighter version of the primary color.
    /// </summary>
    public string PrimaryLighten => AdjustBrightnessForHover(PrimaryColor, false);

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
    public double DefaultFontSize { get; set; } = 14;

     

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
    /// Adjusts the color for UI interaction states (hover, focus) with better visual contrast.
    /// </summary>
    /// <param name="hexColor">The base hex color code.</param>
    /// <param name="isDarkening">True to create a darker variant, false to create a lighter variant.</param>
    /// <returns>The adjusted hex color code optimized for UI interactions.</returns>
    private string AdjustBrightnessForHover(string hexColor, bool isDarkening)
    {
        if (string.IsNullOrWhiteSpace(hexColor))
            throw new ArgumentException("Color code cannot be null or empty.", nameof(hexColor));

        // Parse the hex color using ColorTranslator
        Color color = ColorTranslator.FromHtml(hexColor);

        // Convert RGB to HSL
        ColorToHsl(color, out double h, out double s, out double l);

        if (isDarkening)
        {
            // For darkening (hover states), use a more sophisticated approach
            if (l > 0.8)
            {
                // Very light colors: reduce lightness significantly
                l = Math.Max(l - 0.15, 0.0);
            }
            else if (l > 0.5)
            {
                // Medium-light colors: moderate reduction
                l = Math.Max(l - 0.1, 0.0);
            }
            else if (l > 0.2)
            {
                // Medium-dark colors: small reduction with saturation boost
                l = Math.Max(l - 0.08, 0.0);
                s = Math.Min(s + 0.1, 1.0);
            }
            else
            {
                // Very dark colors: minimal change, slight saturation boost
                l = Math.Max(l - 0.05, 0.0);
                s = Math.Min(s + 0.15, 1.0);
            }
        }
        else
        {
            // For lightening
            if (l < 0.2)
            {
                // Very dark colors: increase lightness significantly
                l = Math.Min(l + 0.2, 1.0);
            }
            else if (l < 0.5)
            {
                // Medium-dark colors: moderate increase
                l = Math.Min(l + 0.15, 1.0);
            }
            else
            {
                // Light colors: small increase with slight saturation reduction
                l = Math.Min(l + 0.1, 1.0);
                s = Math.Max(s - 0.05, 0.0);
            }
        }

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
