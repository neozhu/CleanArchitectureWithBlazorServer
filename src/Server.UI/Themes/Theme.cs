namespace CleanArchitecture.Blazor.Server.UI.Constants;

public class Theme
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
                Warning = "#f0c42b", 
                Error = "#dc3545",
                ErrorContrastText = "#ffffff",
                ErrorDarken = "#b02400",
                ErrorLighten = "#ff5f4a",
                Tertiary = "#20c997",
                Black = "#111", 
                White = "#ffffff", 
                AppbarBackground = "rgba(245, 245, 245, 0.8)",
                AppbarText = "#424242",
                Background = "#f5f5f5", 
                Surface = "#ffffff", 
                DrawerBackground = "#ffffff",
                TextPrimary = "#2E2E2E", 
                TextSecondary = "#6C757D", 
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
                Primary = "#5052ba", 
                Secondary = "#A5A5A5", 
                Success = "#0CAD39", 
                Info = "#4099f3", 
                Warning = "#f0c42b",
                Error = "#dc3545",
                ErrorContrastText = "#ffffff",
                ErrorDarken = "#a30000",
                ErrorLighten = "#ff3333",
                Tertiary = "#20c997",
                Black = "#000000", 
                White = "#ffffff", 
                Background = "#202124", 
                Surface = "#303134",
                AppbarBackground = "rgba(32, 33, 36, 0.8)",
                AppbarText = "rgba(255, 255, 255, 0.7)",
                DrawerBackground = "#303134",
                TextPrimary = "#DADADA", 
                TextSecondary = "#A8A8A8",
                TextDisabled = "rgba(255, 255, 255, 0.3)",
                SecondaryContrastText = "#D5D5D5",
                ActionDefault = "#e8eaed", 
                ActionDisabled = "rgba(255, 255, 255, 0.26)",
                ActionDisabledBackground = "rgba(255, 255, 255, 0.12)",
                Divider = "#3F4452", 
                DividerLight = "rgba(255, 255, 255, 0.06)",
                TableLines = "rgba(63, 68, 82, 0.6)",
                LinesDefault = "#3F4452", 
                LinesInputs = "rgba(255, 255, 255, 0.3)",
            },
            LayoutProperties = new LayoutProperties
            {
                AppbarHeight = "80px",
                DefaultBorderRadius = "6px"
            },
            Typography = new Typography
            {
                Default = new Default
                {
                    FontSize = ".8125rem",
                    FontWeight = 400,
                    LineHeight = 1.4,
                    LetterSpacing = "normal",
                    FontFamily = new[] { "Public Sans", "Roboto", "Arial", "sans-serif" }
                },
                H1 = new H1
                {
                    FontSize = "3.5rem",
                    FontWeight = 700,
                    LineHeight = 1.167,
                    LetterSpacing = "-.01562em"
                },
                H2 = new H2
                {
                    FontSize = "3rem",
                    FontWeight = 300,
                    LineHeight = 1.2,
                    LetterSpacing = "-.00833em"
                },
                H3 = new H3
                {
                    FontSize = "2rem",
                    FontWeight = 600,
                    LineHeight = 1.167,
                    LetterSpacing = "0"
                },
                H4 = new H4
                {
                    FontSize = "1.5rem",
                    FontWeight = 400,
                    LineHeight = 1.235,
                    LetterSpacing = ".00735em"
                },
                H5 = new H5
                {
                    FontSize = "1.25rem",
                    FontWeight = 400,
                    LineHeight = 1.3,
                    LetterSpacing = "0"
                },
                H6 = new H6
                {
                    FontSize = "1.125rem",
                    FontWeight = 600,
                    LineHeight = 1.5,
                    LetterSpacing = ".0075em"
                },
                Button = new Button
                {
                    FontSize = ".8125rem",
                    FontWeight = 500,
                    LineHeight = 1.75,
                    LetterSpacing = ".02857em",
                    TextTransform = "uppercase"
                },
                Subtitle1 = new Subtitle1
                {
                    FontSize = "1rem",
                    FontWeight = 600,
                    LineHeight = 1.75,
                    LetterSpacing = ".00938em",
                    TextTransform = "none"
                },
                Subtitle2 = new Subtitle2
                {
                    FontSize = ".875rem",
                    FontWeight = 500,
                    LineHeight = 1.57,
                    LetterSpacing = ".00714em"
                },
                Body1 = new Body1
                {
                    FontSize = "0.875rem",
                    FontWeight = 400,
                    LineHeight = 1.5,
                    LetterSpacing = ".00938em"
                },
                Body2 = new Body2
                {
                    FontSize = ".8125rem",
                    FontWeight = 400,
                    LineHeight = 1.43,
                    LetterSpacing = ".01071em"
                },
                Caption = new Caption
                {
                    FontSize = ".75rem",
                    FontWeight = 400,
                    LineHeight = 1.66,
                    LetterSpacing = ".03333em"
                },
                Overline = new Overline
                {
                    FontSize = ".75rem",
                    FontWeight = 400,
                    LineHeight = 2.5,
                    LetterSpacing = ".08333em"
                }
            },
            Shadows = new Shadow
            {
                Elevation = new[]
                {
                    "none",
                    "0 2px 4px -1px rgba(6, 24, 44, 0.2)",
                    "0px 3px 1px -2px rgba(0,0,0,0.2),0px 2px 2px 0px rgba(0,0,0,0.14),0px 1px 5px 0px rgba(0,0,0,0.12)",
                    "0 30px 60px rgba(0,0,0,0.12)",
                    "0 6px 12px -2px rgba(50,50,93,0.25),0 3px 7px -3px rgba(0,0,0,0.3)",
                    "0 50px 100px -20px rgba(50,50,93,0.25),0 30px 60px -30px rgba(0,0,0,0.3)",
                    "0px 3px 5px -1px rgba(0,0,0,0.2),0px 6px 10px 0px rgba(0,0,0,0.14),0px 1px 18px 0px rgba(0,0,0,0.12)",
                    "0px 4px 5px -2px rgba(0,0,0,0.2),0px 7px 10px 1px rgba(0,0,0,0.14),0px 2px 16px 1px rgba(0,0,0,0.12)",
                    "0px 5px 5px -3px rgba(0,0,0,0.2),0px 8px 10px 1px rgba(0,0,0,0.14),0px 3px 14px 2px rgba(0,0,0,0.12)",
                    "0px 5px 6px -3px rgba(0,0,0,0.2),0px 9px 12px 1px rgba(0,0,0,0.14),0px 3px 16px 2px rgba(0,0,0,0.12)",
                    "0px 6px 6px -3px rgba(0,0,0,0.2),0px 10px 14px 1px rgba(0,0,0,0.14),0px 4px 18px 3px rgba(0,0,0,0.12)",
                    "0px 6px 7px -4px rgba(0,0,0,0.2),0px 11px 15px 1px rgba(0,0,0,0.14),0px 4px 20px 3px rgba(0,0,0,0.12)",
                    "0px 7px 8px -4px rgba(0,0,0,0.2),0px 12px 17px 2px rgba(0,0,0,0.14),0px 5px 22px 4px rgba(0,0,0,0.12)",
                    "0px 7px 8px -4px rgba(0,0,0,0.2),0px 13px 19px 2px rgba(0,0,0,0.14),0px 5px 24px 4px rgba(0,0,0,0.12)",
                    "0px 7px 9px -4px rgba(0,0,0,0.2),0px 14px 21px 2px rgba(0,0,0,0.14),0px 5px 26px 4px rgba(0,0,0,0.12)",
                    "0px 8px 9px -5px rgba(0,0,0,0.2),0px 15px 22px 2px rgba(0,0,0,0.14),0px 6px 28px 5px rgba(0,0,0,0.12)",
                    "0px 8px 10px -5px rgba(0,0,0,0.2),0px 16px 24px 2px rgba(0,0,0,0.14),0px 6px 30px 5px rgba(0,0,0,0.12)",
                    "0px 8px 11px -5px rgba(0,0,0,0.2),0px 17px 26px 2px rgba(0,0,0,0.14),0px 6px 32px 5px rgba(0,0,0,0.12)",
                    "0px 9px 11px -5px rgba(0,0,0,0.2),0px 18px 28px 2px rgba(0,0,0,0.14),0px 7px 34px 6px rgba(0,0,0,0.12)",
                    "0px 9px 12px -6px rgba(0,0,0,0.2),0px 19px 29px 2px rgba(0,0,0,0.14),0px 7px 36px 6px rgba(0,0,0,0.12)",
                    "0px 10px 13px -6px rgba(0,0,0,0.2),0px 20px 31px 3px rgba(0,0,0,0.14),0px 8px 38px 7px rgba(0,0,0,0.12)",
                    "0px 10px 13px -6px rgba(0,0,0,0.2),0px 21px 33px 3px rgba(0,0,0,0.14),0px 8px 40px 7px rgba(0,0,0,0.12)",
                    "0px 10px 14px -6px rgba(0,0,0,0.2),0px 22px 35px 3px rgba(0,0,0,0.14),0px 8px 42px 7px rgba(0,0,0,0.12)",
                    "0 50px 100px -20px rgba(50, 50, 93, 0.25), 0 30px 60px -30px rgba(0, 0, 0, 0.30)",
                    "2.8px 2.8px 2.2px rgba(0, 0, 0, 0.02),6.7px 6.7px 5.3px rgba(0, 0, 0, 0.028),12.5px 12.5px 10px rgba(0, 0, 0, 0.035),22.3px 22.3px 17.9px rgba(0, 0, 0, 0.042),41.8px 41.8px 33.4px rgba(0, 0, 0, 0.05),100px 100px 80px rgba(0, 0, 0, 0.07)",
                    "0px 0px 20px 0px rgba(0, 0, 0, 0.05)"
                }
            }
        };
        return theme;
    }
}