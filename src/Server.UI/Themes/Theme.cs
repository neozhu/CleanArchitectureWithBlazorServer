namespace CleanArchitecture.Blazor.Server.UI.Themes;

public static class Theme
{
    public static MudTheme ApplicationTheme()
    {
        var theme = new MudTheme
        {
            PaletteLight = new PaletteLight
            {
                Primary = "#0f172a", // Modern blue, professional and trustworthy
                PrimaryContrastText = "#ffffff",
                PrimaryDarken = "#020617",
                PrimaryLighten = "#1e293b",
                Secondary = "#71717a",
                SecondaryContrastText = "#ffffff",
                SecondaryLighten = "#52525b",
                SecondaryDarken = "#a1a1aa",
                Success = "#10b981", // Fresh green, success
                Info = "#0ea5e9", // Info blue, clear
                Tertiary = "#8b5cf6",              // Purple 500
                TertiaryContrastText = "#ffffff",
                TertiaryDarken = "#7c3aed",        // Purple 600
                TertiaryLighten = "#a78bfa",       // Purple 400

                Warning = "#f59e0b",               // Amber 500
                WarningContrastText = "#92400e",   // Amber 800
                WarningDarken = "#d97706",         // Amber 600
                WarningLighten = "#fbbf24",        // Amber 400

                Error = "#dc2626", // Clear red, error
                ErrorContrastText = "#ffffff",
                ErrorDarken = "#b91c1c",
                ErrorLighten = "#ef4444",

                Black = "#020617", // Deep blue-black, more texture
                White = "#ffffff",
                AppbarBackground = "#f8fafc", // Very light blue-gray, modern
                AppbarText = "#0a0a0a",
                Background = "#f8fafc", // Very light blue-gray, modern
                Surface = "#ffffff",
                DrawerBackground = "#ffffff",
                TextPrimary = "#0f172a", // Deep blue-gray, modern professional
                TextSecondary = "#64748b", // Neutral gray, hierarchy

                DrawerIcon = "#71717a",

                TextDisabled = "#94a3b8", // Soft gray
                ActionDefault = "#262626",
                ActionDisabled = "rgba(100, 116, 139, 0.4)",
                ActionDisabledBackground = "rgba(100, 116, 139, 0.1)",
                Divider = "#e2e8f0", // Elegant divider
                DividerLight = "#f1f5f9",
                TableLines = "#e2e8f0", // Table lines, elegant
                LinesDefault = "#e2e8f0",
                LinesInputs = "#cbd5e1",
            },
            PaletteDark = new PaletteDark
            {
                Primary = "#fafafa", // shadcn/ui white primary
                PrimaryContrastText = "#020817",
                PrimaryDarken = "#e4e4e7",
                PrimaryLighten = "#ffffff",
                Secondary = "#78716c", // Neutral gray
                Success = "#22c55e", // Green for success
                Info = "#0ea5e9", // Sky blue for info (shadcn sky-500)
                InfoDarken = "#0284c7", // Darker sky blue (shadcn sky-600)
                InfoLighten = "#38bdf8", // Lighter sky blue (shadcn sky-400)

                Tertiary = "#6366f1",
                TertiaryContrastText = "#fafafa",
                TertiaryDarken = "#4f46e5",
                TertiaryLighten = "#818cf8",

                Warning = "#f59e0b", // Orange for warning
                WarningContrastText = "#fafafa",
                WarningDarken = "#d97706",
                WarningLighten = "#fbbf24",

                Error = "#ef4444", // Red for error
                ErrorContrastText = "#fafafa",
                ErrorDarken = "#dc2626",
                ErrorLighten = "#f87171",

                Black = "#020817",
                White = "#fafafa",
                Background = "#0c0a09", // shadcn/ui dark background
                Surface = "#171717", // Deeper surface color
                AppbarBackground = "#0c0a09",
                AppbarText = "#fafafa",
                DrawerText = "#fafafa",
                DrawerIcon = "#a1a1aa",

                DrawerBackground = "#0c0a09",
                TextPrimary = "#fafafa", // shadcn/ui white text
                TextSecondary = "#a1a1aa", // Neutral gray secondary text
                TextDisabled = "rgba(161, 161, 170, 0.5)",
                ActionDefault = "#e5e5e5",
                ActionDisabled = "rgba(161, 161, 170, 0.3)",
                ActionDisabledBackground = "rgba(161, 161, 170, 0.1)",
                Divider = "rgba(255, 255, 255, 0.1)", // shadcn/ui divider color
                DividerLight = "rgba(161, 161, 170, 0.1)",
                TableLines = "rgba(255, 255, 255, 0.1)",
                LinesDefault = "rgba(255, 255, 255, 0.1)",
                LinesInputs = "rgba(161, 161, 170, 0.2)",
                DarkContrastText = "#020817",
                SecondaryContrastText = "#fafafa",
                SecondaryDarken = "#57534e",
                SecondaryLighten = "#a8a29e"

            },
            LayoutProperties = new LayoutProperties
            {
                AppbarHeight = "64px", // More modern height
                DefaultBorderRadius = "8px", // More modern border radius
                DrawerWidthLeft = "280px", // Wider sidebar
                DrawerMiniWidthRight = "260px"
            },
            Typography = new Typography
            {
                Default = new DefaultTypography
                {
                    FontSize = ".875rem",
                    FontWeight = "400",
                    LineHeight = "1.43",
                    LetterSpacing = "normal",
                    FontFamily = [
    "Inter",
    "-apple-system",
    "BlinkMacSystemFont",
    "Segoe UI",
    "PingFang SC",
    "Microsoft YaHei UI",
    "Microsoft YaHei",
    "sans-serif"
]
                },
                H1 = new H1Typography
                {
                    FontSize = "2.5rem",
                    FontWeight = "700",
                    LineHeight = "3rem",
                    LetterSpacing = "-0.025em"
                },
                H2 = new H2Typography
                {
                    FontSize = "2rem",
                    FontWeight = "600",
                    LineHeight = "2.5rem",
                    LetterSpacing = "-0.025em"
                },
                H3 = new H3Typography
                {
                    FontSize = "1.75rem",
                    FontWeight = "600",
                    LineHeight = "2rem",
                    LetterSpacing = "-0.025em"
                },
                H4 = new H4Typography
                {
                    FontSize = "1.5rem",
                    FontWeight = "600",
                    LineHeight = "1.75rem",
                    LetterSpacing = "-0.025em"
                },
                H5 = new H5Typography
                {
                    FontSize = "1.25rem",
                    FontWeight = "600",
                    LineHeight = "1.75rem",
                    LetterSpacing = "normal"
                },
                H6 = new H6Typography
                {
                    FontSize = "1rem",
                    FontWeight = "600",
                    LineHeight = "1.5rem",
                    LetterSpacing = "normal"
                },
                Button = new ButtonTypography
                {
                    FontSize = ".875rem",
                    FontWeight = "500",
                    LineHeight = "1.75rem",
                    LetterSpacing = "0.01em",
                    TextTransform = "none"
                },
                Subtitle1 = new Subtitle1Typography
                {
                    FontSize = "1rem",
                    FontWeight = "500",
                    LineHeight = "1.75rem",
                    LetterSpacing = ".00714em",
                },
                Subtitle2 = new Subtitle2Typography
                {
                    FontSize = ".875rem",
                    FontWeight = "400",
                    LineHeight = "1.5rem",
                    LetterSpacing = "normal"
                },
                Body1 = new Body1Typography
                {
                    FontSize = ".875rem",
                    FontWeight = "400",
                    LineHeight = "1.5rem",
                    LetterSpacing = "normal"
                },
                Body2 = new Body2Typography
                {
                    FontSize = ".75rem",
                    FontWeight = "400",
                    LineHeight = "1.25rem",
                    LetterSpacing = "0.01em"
                },
                Caption = new CaptionTypography
                {
                    FontSize = "0.75rem",
                    FontWeight = "400",
                    LineHeight = "1.25rem",
                    LetterSpacing = "0.02em"

                },
                Overline = new OverlineTypography
                {
                    FontSize = "0.75rem",
                    FontWeight = "500",
                    LineHeight = "1.5rem",
                    LetterSpacing = "0.08em",
                    TextTransform = "uppercase"
                }
            }
        };
        return theme;
    }
}