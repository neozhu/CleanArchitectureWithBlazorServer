namespace CleanArchitecture.Blazor.Server.UI.Themes;

public static class Theme
{
    public static MudTheme ApplicationTheme()
    {
        var theme = new MudTheme
        {
            PaletteLight = new PaletteLight
            {
                Primary = "#2d4275", // Modern blue, professional and trustworthy
                Secondary = "#6b7280", // Neutral gray, clean and professional
                Success = "#10b981", // Fresh green, success
                Info = "#0ea5e9", // Info blue, clear
                Warning = "#f59e0b", // Gentle orange, warning
                Error = "#ef4444", // Clear red, error
                ErrorContrastText = "#ffffff",
                ErrorDarken = "#dc2626",
                ErrorLighten = "#f87171",
                Tertiary = "#8b5cf6", // Modern purple, tech sense
                Black = "#0f172a", // Deep blue-black, more texture
                White = "#ffffff", 
                AppbarBackground = "#f8fafc", // Very light blue-gray, modern
                AppbarText = "#0a0a0a",
                Background = "#f8fafc", // Very light blue-gray, modern
                Surface = "#ffffff", 
                DrawerBackground = "#ffffff",
                TextPrimary = "#0a0a0a", // Deep blue-gray, modern professional
                TextSecondary = "#737373", // Neutral gray, hierarchy
                SecondaryContrastText = "#ffffff", 
                TextDisabled = "#94a3b8", // Soft gray
                ActionDefault = "#64748b", 
                ActionDisabled = "rgba(100, 116, 139, 0.4)",
                ActionDisabledBackground = "rgba(100, 116, 139, 0.1)",
                Divider = "#e5e5e5", // Elegant divider
                DividerLight = "rgba(100, 116, 139, 0.12)",
                TableLines = "#e5e5e5", // Table lines, elegant
                LinesDefault = "#e2e8f0", 
                LinesInputs = "#d1d5db",
            },
            PaletteDark = new PaletteDark
            {
                Primary = "#fafafa", // shadcn/ui white primary
                Secondary = "#71717a", // Neutral gray
                Success = "#22c55e", // Green for success
                Info = "#06b6d4", // Cyan for info
                Warning = "#f59e0b", // Orange for warning
                Error = "#ef4444", // Red for error
                ErrorContrastText = "#fafafa",
                ErrorDarken = "#dc2626",
                ErrorLighten = "#f87171",
                Tertiary = "#a855f7", // Purple
                Black = "#020817", 
                White = "#fafafa", 
                Background = "#0c0a09", // shadcn/ui dark background
                Surface = "#171717", // Deeper surface color
                AppbarBackground = "#0c0a09",
                AppbarText = "#fafafa",
                DrawerText = "#fafafa",
                DrawerBackground = "#0c0a09",
                TextPrimary = "#e5e5e5", // shadcn/ui white text
                TextSecondary = "#a1a1aa", // Neutral gray secondary text
                TextDisabled = "rgba(161, 161, 170, 0.5)",
                ActionDefault = "#a1a1aa", 
                ActionDisabled = "rgba(161, 161, 170, 0.3)",
                ActionDisabledBackground = "rgba(161, 161, 170, 0.1)",
                Divider = "rgba(255, 255, 255, 0.1)", // shadcn/ui divider color
                DividerLight = "rgba(161, 161, 170, 0.1)",
                TableLines = "rgba(255, 255, 255, 0.1)",
                LinesDefault = "rgba(255, 255, 255, 0.1)", 
                LinesInputs = "rgba(161, 161, 170, 0.2)",
                DarkContrastText= "#020817",
                SecondaryContrastText = "#fafafa",
                PrimaryContrastText = "#020817",
            },
            LayoutProperties = new LayoutProperties
            {
                AppbarHeight = "64px", // More modern height
                DefaultBorderRadius = "8px", // More modern border radius
                DrawerWidthLeft = "280px", // Wider sidebar
            },
            Typography = new Typography
            {
                Default = new DefaultTypography
                {
                    FontSize = ".875rem", // 14px, modern standard
                    FontWeight = "400",
                    LineHeight = "1.5",
                    LetterSpacing = "normal",
                    FontFamily = ["Inter", "system-ui", "-apple-system", "BlinkMacSystemFont", "Segoe UI", "Roboto", "sans-serif"]
                },
                H1 = new H1Typography
                {
                    FontSize = "2.25rem", // 36px   
                    FontWeight = "700",        
                    LineHeight = "1.2",       
                    LetterSpacing = "-0.025em"
                },
                H2 = new H2Typography
                {
                    FontSize = "1.875rem", // 30px    
                    FontWeight = "600",
                    LineHeight = "1.25",
                    LetterSpacing = "-0.025em"
                },
                H3 = new H3Typography
                {
                    FontSize = "1.5rem", // 24px     
                    FontWeight = "600",
                    LineHeight = "1.33",
                    LetterSpacing = "-0.025em"
                },
                H4 = new H4Typography
                {
                    FontSize = "1.25rem", // 20px     
                    FontWeight = "600",
                    LineHeight = "1.4",
                    LetterSpacing = "-0.025em"
                },
                H5 = new H5Typography
                {
                    FontSize = "1.125rem", // 18px    
                    FontWeight = "600",
                    LineHeight = "1.44",
                    LetterSpacing = "-0.025em"
                },
                H6 = new H6Typography
                {
                    FontSize = "1rem", // 16px       
                    FontWeight = "600",
                    LineHeight = "1.5",
                    LetterSpacing = "-0.025em"
                },
                Button = new ButtonTypography
                {
                    FontSize = ".875rem", // 14px
                    FontWeight = "500",
                    LineHeight = "1.5",
                    LetterSpacing = ".025em",
                    TextTransform = "none" // Modern design does not use uppercase
                },
                Subtitle1 = new Subtitle1Typography
                {
                    FontSize = "1rem", // 16px
                    FontWeight = "500",
                    LineHeight = "1.5",
                    LetterSpacing = "-0.025em",
                },
                Subtitle2 = new Subtitle2Typography
                {
                    FontSize = ".875rem", // 14px
                    FontWeight = "500",
                    LineHeight = "1.43",
                    LetterSpacing = "-0.025em"
                },
                Body1 = new Body1Typography
                {
                    FontSize = "1rem", // 16px
                    FontWeight = "400",
                    LineHeight = "1.5",
                    LetterSpacing = "-0.025em"
                },
                Body2 = new Body2Typography
                {
                    FontSize = ".875rem", // 14px
                    FontWeight = "400",
                    LineHeight = "1.43",
                    LetterSpacing = "-0.025em"
                },
                Caption = new CaptionTypography
                {
                    FontSize = "0.75rem", // 12px
                    FontWeight = "500",
                    LineHeight = "1.33",
                    LetterSpacing = "0.025em"
                },
                Overline = new OverlineTypography
                {
                    FontSize = "0.75rem", // 12px
                    FontWeight = "500",
                    LineHeight = "1.33",
                    LetterSpacing = "0.1em",
                    TextTransform = "uppercase"
                }
            }
        };
        return theme;
    }
}