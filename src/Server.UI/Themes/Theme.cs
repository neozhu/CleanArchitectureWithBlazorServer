namespace CleanArchitecture.Blazor.Server.UI.Themes;

public static class Theme
{
    public static MudTheme ApplicationTheme()
    {
        var theme = new MudTheme
        {
            PaletteLight = new PaletteLight
            {
                Primary = "#5052ba", 
                Secondary = "#7D7D7D", 
                Success = "#0CAD39", 
                Info = "#4099f3", 
                Warning = "#f54a00", 
                Error = "rgba(244,67,54,1)",
                ErrorContrastText = "#ffffff",
                ErrorDarken = "rgb(242,28,13)",
                ErrorLighten = "rgb(246,96,85)",
                Tertiary = "#20c997",
                Black = "#111", 
                White = "#ffffff", 
                AppbarBackground = "rgba(245, 245, 245, 0.8)",
                AppbarText = "#424242",
                Background = "#f5f5f5", 
                Surface = "#ffffff", 
                DrawerBackground = "#ffffff",
                TextPrimary = "#000000", 
                TextSecondary = "#757575", 
                SecondaryContrastText = "#F5F5F5", 
                TextDisabled = "#B0B0B0", 
                ActionDefault = "#80838b", 
                ActionDisabled = "rgba(128, 131, 139, 0.3)",
                ActionDisabledBackground = "rgba(128, 131, 139, 0.12)",
                Divider = "#e2e5e8", 
                DividerLight = "rgba(128, 131, 139, 0.15)",
                TableLines = "#eff0f2", 
                LinesDefault = "#e2e5e8", 
                LinesInputs = "#e2e5e8",
               

            },
            PaletteDark = new PaletteDark
            {
                Primary = "#0170f3", 
                Secondary = "#A5A5A5", 
                Success = "#0CAD39", 
                Info = "#4099f3", 
                Warning = "#ff6800",
                Error = "#f33f33",
                ErrorContrastText = "#ffffff",
                ErrorDarken = "#e02d48",
                ErrorLighten = "#ff3333",
                Tertiary = "#ff0080",
                Black = "#000000", 
                White = "#ffffff", 
                Background = "#111111", 
                Surface = "#222222",
                AppbarBackground = "rgba(17,17,17,0.8)",
                AppbarText = "rgba(255, 255, 255, 0.75)",
                DrawerText = "rgba(255, 255, 255, 0.75)",
                DrawerBackground = "#222222",
                TextPrimary = "#DADADA", 
                TextSecondary = "#A6A6A6",
                TextDisabled = "rgba(255, 255, 255, 0.38)",
                ActionDefault = "#e8eaed", 
                ActionDisabled = "rgba(255, 255, 255, 0.26)",
                ActionDisabledBackground = "rgba(255, 255, 255, 0.12)",
                Divider = "#333333", 
                DividerLight = "rgba(255, 255, 255, 0.06)",
                TableLines = "rgba(63, 68, 82, 0.6)",
                LinesDefault = "#333333", 
                LinesInputs = "rgba(255, 255, 255, 0.3)",
                DarkContrastText= "#FAFAFA",
                SecondaryContrastText = "#D5D5D5",
                PrimaryContrastText = "#FAFAFA",
                
            },
            LayoutProperties = new LayoutProperties
            {
                AppbarHeight = "80px",
                DefaultBorderRadius = "6px",
                DrawerWidthLeft = "256px",
            },
            Typography = new Typography
            {
                Default = new DefaultTypography
                {
                    FontSize = ".8125rem",
                    FontWeight = "400",
                    LineHeight = "1.4",
                    LetterSpacing = "normal",
                    FontFamily = ["Public Sans", "Roboto", "Arial", "sans-serif"]
                },
                H1 = new H1Typography
                {
                    FontSize = "2.25rem",    
                    FontWeight = "700",        
                    LineHeight = "2.5",       
                },
                H2 = new H2Typography
                {
                    FontSize = "1.875rem",     
                    FontWeight = "700",
                    LineHeight = "2.25",
                },
                H3 = new H3Typography
                {
                    FontSize = "1.5rem",      
                    FontWeight = "700",
                    LineHeight = "2",
                },
                H4 = new H4Typography
                {
                    FontSize = "1.25rem",      
                    FontWeight = "700",
                    LineHeight = "1.75",
                },
                H5 = new H5Typography
                {
                    FontSize = "1.125rem",     
                    FontWeight = "600",
                    LineHeight = "1.75",
                },
                H6 = new H6Typography
                {
                    FontSize = "1rem",        
                    FontWeight = "600",
                    LineHeight = "1.5",
                },
                Button = new ButtonTypography
                {
                    FontSize = ".8125rem",
                    FontWeight = "500",
                    LineHeight = "1.75",
                    LetterSpacing = ".02857em",
                    TextTransform = "uppercase"
                },
                Subtitle1 = new Subtitle1Typography
                {
                    FontSize = ".8125rem",
                    FontWeight = "400",
                    LineHeight = "1.5",
                    LetterSpacing = "normal",
                },
                Subtitle2 = new Subtitle2Typography
                {
                    FontSize = ".875rem",
                    FontWeight = "500",
                    LineHeight = "1.57",
                },
                Body1 = new Body1Typography
                {
                    FontSize = "0.8125rem",
                    FontWeight = "400",
                    LineHeight = "1.5",
                },
                Body2 = new Body2Typography
                {
                    FontSize = ".75rem",
                    FontWeight = "400",
                    LineHeight = "1.4",
                },
                Caption = new CaptionTypography
                {
                    FontSize = "0.625rem",
                    FontWeight = "400",
                    LineHeight = "1.4",
                    LetterSpacing = "normal"
                },
                Overline = new OverlineTypography
                {
                    FontSize = "0.625rem",
                    FontWeight = "300",
                    LineHeight = "1.5",
                    LetterSpacing = "0.1em"
                }
            }
        };
        return theme;
    }
}