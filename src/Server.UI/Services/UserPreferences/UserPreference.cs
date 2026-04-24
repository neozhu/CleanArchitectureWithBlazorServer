// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.Globalization;

namespace CleanArchitecture.Blazor.Server.UI.Services.UserPreferences;

public class UserPreference
{
    // List of available primary colors (Updated to Tailwind 600 scale for better white text contrast)
    public static readonly List<string> PrimaryColors = new()
    {
        "#0f172a", // Slate-900
        "#2563eb", // Blue-600
        "#7c3aed", // Violet-600
        "#059669", // Emerald-600
        "#ea580c", // Orange-600
        "#e11d48", // Rose-600
    };

    // List of available dark primary colors (Tailwind 500 scale or White)
    public static readonly List<string> DarkPrimaryColors = new()
    {
        "#fafafa", // Zinc-50
        "#3b82f6", // Blue-500
        "#8b5cf6", // Violet-500
        "#10b981", // Emerald-500
        "#f97316", // Orange-500
        "#f43f5e", // Rose-500
    };

    public bool IsDarkMode { get; set; }
    public bool RightToLeft { get; set; }

    // Default assignments
    public string PrimaryColor { get; set; } = PrimaryColors[0];
    public string DarkPrimaryColor { get; set; } = DarkPrimaryColors[0];
    public double BorderRadius { get; set; } = 4;
    public double DefaultFontSize { get; set; } = 15;
    public DarkLightMode DarkLightTheme { get; set; }

    /// <summary>
    /// Static map defining the interaction states (Contrast, Darken, Lighten) for each primary color.
    /// </summary>
    private static readonly Dictionary<string, ColorConfig> ColorMap = new()
    {
        // ----------------------------------------------------------------------
        // Light Mode Mappings (Base: 600 series -> Darken: 700 series, Lighten: 500 series)
        // Note: White text (#ffffff) is used for all 600-series colors.
        // ----------------------------------------------------------------------
        
        // Slate-900 -> D: Slate-950, L: Slate-800
        { "#0f172a", new ColorConfig("#ffffff", "#020617", "#1e293b") }, 
        
        // Blue-600 -> D: Blue-700, L: Blue-500
        { "#2563eb", new ColorConfig("#ffffff", "#1d4ed8", "#3b82f6") }, 
        
        // Violet-600 -> D: Violet-700, L: Violet-500
        { "#7c3aed", new ColorConfig("#ffffff", "#6d28d9", "#8b5cf6") }, 
        
        // Emerald-600 -> D: Emerald-700, L: Emerald-500
        { "#059669", new ColorConfig("#ffffff", "#047857", "#10b981") }, 
        
        // Orange-600 -> D: Orange-700, L: Orange-500
        { "#ea580c", new ColorConfig("#ffffff", "#c2410c", "#f97316") }, 
        
        // Rose-600 -> D: Rose-700, L: Rose-500
        { "#e11d48", new ColorConfig("#ffffff", "#be123c", "#f43f5e") }, 


        // ----------------------------------------------------------------------
        // Dark Mode Mappings (Base: 500 series/White -> Darken: 600 series/Gray, Lighten: 400 series/White)
        // ----------------------------------------------------------------------

        // Zinc-50 (White) -> D: Zinc-200, L: Pure White. *Contrast Text is Black*.
        { "#fafafa", new ColorConfig("#020817", "#e4e4e7", "#ffffff") }, 
        
        // Blue-500 -> D: Blue-600, L: Blue-400
        { "#3b82f6", new ColorConfig("#ffffff", "#2563eb", "#60a5fa") }, 
        
        // Violet-500 -> D: Violet-600, L: Violet-400
        { "#8b5cf6", new ColorConfig("#ffffff", "#7c3aed", "#a78bfa") }, 
        
        // Emerald-500 -> D: Emerald-600, L: Emerald-400
        { "#10b981", new ColorConfig("#ffffff", "#059669", "#34d399") }, 
        
        // Orange-500 -> D: Orange-600, L: Orange-400
        { "#f97316", new ColorConfig("#ffffff", "#ea580c", "#fb923c") }, 
        
        // Rose-500 -> D: Rose-600, L: Rose-400
        { "#f43f5e", new ColorConfig("#ffffff", "#e11d48", "#fb7185") }
    };



    // -------------------------------------------------------------------------
    // Optimized Color Logic
    // -------------------------------------------------------------------------

    public string PrimaryDarken
    {
        get
        {
            var currentColor = IsDarkMode ? DarkPrimaryColor : PrimaryColor;
            if (ColorMap.TryGetValue(currentColor, out var config))
            {
                return config.Darken;
            }
            return (currentColor == "#fafafa" || currentColor == "#ffffff") ? "#e4e4e7" : currentColor;
        }
    }
    public string PrimaryLighten
    {
        get
        {
            var currentColor = IsDarkMode ? DarkPrimaryColor : PrimaryColor;
            if (ColorMap.TryGetValue(currentColor, out var config))
            {
                return config.Lighten;
            }
            return (currentColor == "#fafafa" || currentColor == "#ffffff") ? "#ffffff" : currentColor;
        }
    }
    public string PrimaryContrastText
    {
        get
        {
            var currentColor = IsDarkMode ? DarkPrimaryColor : PrimaryColor;
            if (ColorMap.TryGetValue(currentColor, out var config))
            {
                return config.ContrastText;
            }
            return currentColor == "#fafafa" || currentColor == "#ffffff" ? "#020817" : "#ffffff";
        }
    }

    /// <summary>
    /// Configuration record for color states.
    /// </summary>
    /// <param name="ContrastText">The text color to use on top of the primary color.</param>
    /// <param name="Darken">The darker shade for hover states.</param>
    /// <param name="Lighten">The lighter shade for ripple/active states.</param>
    private record ColorConfig(string ContrastText, string Darken, string Lighten);
}
public enum DarkLightMode
{
    System = 0,
    Light = 1,
    Dark = 2
}
