namespace CleanArchitecture.Blazor.Server.UI.Themes;

public static class Theme
{
    public static MudTheme ApplicationTheme()
    {
        var theme = new MudTheme
        {
            PaletteLight = new PaletteLight
            {
                Primary = "#172554", // Modern blue, professional and trustworthy
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
                ActionDefault = "#262626", 
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
                TextPrimary = "#d4d4d4", // shadcn/ui white text
                TextSecondary = "#a1a1aa", // Neutral gray secondary text
                TextDisabled = "rgba(161, 161, 170, 0.5)",
                ActionDefault = "#d4d4d4", 
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
                DrawerMiniWidthRight= "260px"
            },
            Typography = new Typography
            {
                Default = new DefaultTypography
                {
                    FontSize = ".875rem",
                    FontWeight = "400",
                    LineHeight = "1.43",
                    LetterSpacing = "normal",
                    FontFamily = ["Inter var", "Inter", "ui-sans-serif", "system-ui", "-apple-system", "Segoe UI", "Roboto", "Helvetica Neue", "Arial", "Noto Sans", "sans-serif", "Apple Color Emoji", "Segoe UI Emoji"]
                },
                H1 = new H1Typography
                {
                    FontSize = "2.25rem", 
                    FontWeight = "800",        
                    LineHeight = "2.5rem",       
                    LetterSpacing = "-0.025em"
                },
                H2 = new H2Typography
                {
                    FontSize = "1.875rem",
                    FontWeight = "600",
                    LineHeight = "2.25rem",
                    LetterSpacing = "-0.025em"
                },
                H3 = new H3Typography
                {
                    FontSize = "1.5rem", 
                    FontWeight = "600",
                    LineHeight = "2rem",
                    LetterSpacing = "-0.025em"
                },
                H4 = new H4Typography
                {
                    FontSize = "1.25rem",
                    FontWeight = "600",
                    LineHeight = "1.75rem",
                    LetterSpacing = "-0.025em"
                },
                H5 = new H5Typography
                {
                    FontSize = "1.125rem",
                    FontWeight = "600",
                    LineHeight = "1.75rem",
                    LetterSpacing = "-0.025em"
                },
                H6 = new H6Typography
                {
                    FontSize = "1rem",
                    FontWeight = "600",
                    LineHeight = "1.25rem",
                    LetterSpacing = "-0.025em"
                },
                Button = new ButtonTypography
                {
                    FontSize = ".875rem",
                    FontWeight = "500",
                    LineHeight = "1.75rem",
                    LetterSpacing = "normal",
                    TextTransform = "none"
                },
                Subtitle1 = new Subtitle1Typography
                {
                    FontSize = ".875rem",
                    FontWeight = "400",
                    LineHeight = "1.5rem",
                    LetterSpacing = ".00938em",
                },
                Subtitle2 = new Subtitle2Typography
                {
                    FontSize = "1rem",
                    FontWeight = "500",
                    LineHeight = "1.75rem",
                    LetterSpacing = ".00714em"
                },
                Body1 = new Body1Typography
                {
                    FontSize = ".875rem",
                    FontWeight = "400",
                    LineHeight = "1.5rem",
                    LetterSpacing = ".00938em"
                },
                Body2 = new Body2Typography
                {
                    FontSize = ".75rem",
                    FontWeight = "400",
                    LineHeight = "1.25rem",
                    LetterSpacing = ".01071em"
                },
                Caption = new CaptionTypography
                {
                    FontSize = "0.75rem", 
                    FontWeight = "400",
                    LineHeight = "1.5rem",
                    LetterSpacing = ".03333em"
                },
                Overline = new OverlineTypography
                {
                    FontSize = "0.75rem",
                    FontWeight = "400",
                    LineHeight = "1.75rem",
                    LetterSpacing = ".03333em",
                    TextTransform = "none"
                }
            }
        };
        return theme;
    }
}
