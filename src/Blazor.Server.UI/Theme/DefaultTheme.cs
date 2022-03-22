using MudBlazor;

namespace Blazor.Server.UI;

public class Theme
{
    public static MudTheme ApplicationTheme()
    {
       var theme = new MudTheme()
        {

            Palette = new Palette
            {
                Primary = "#2d4275",
                Black = "#0A0E19",
                Success = "#64A70B",
                Secondary = "#ff4081ff",
                AppbarBackground = "rgba(255,255,255,0.8)",
                AppbarText = "#424242",
                BackgroundGrey = "#F9FAFC",
                TextSecondary = "#425466",
                Dark = "#110E2D",
                DarkLighten = "#1A1643",
                GrayDefault = "#4B5563",
                GrayLight = "#9CA3AF",
                GrayLighter = "#adbdccff"
            },
            PaletteDark = new Palette
            {
                Primary = "#7e6fff",
                Black = "#27272f",
                Background = "rgb(21,27,34)",
                BackgroundGrey = "#27272f",
                Surface = "#212B36",
                DrawerBackground = "rgb(21,27,34)",
                DrawerText = "rgba(255,255,255, 0.50)",
                DrawerIcon = "rgba(255,255,255, 0.50)",
                AppbarBackground = "rgba(21,27,34,0.7)",
                AppbarText = "rgba(255,255,255, 0.70)",
                TextPrimary = "rgba(255,255,255, 0.70)",
                TextSecondary = "rgba(255,255,255, 0.50)",
                ActionDefault = "#adadb1",
                ActionDisabled = "rgba(255,255,255, 0.26)",
                ActionDisabledBackground = "rgba(255,255,255, 0.12)",
                DarkDarken = "rgba(21,27,34,0.7)",
                Divider = "rgba(255,255,255, 0.12)",
                DividerLight = "rgba(255,255,255, 0.06)",
                TableLines = "rgba(255,255,255, 0.12)",
                LinesDefault = "rgba(255,255,255, 0.12)",
                LinesInputs = "rgba(255,255,255, 0.3)",
                TextDisabled = "rgba(255,255,255, 0.2)"
            },
            LayoutProperties = new LayoutProperties
            {
                AppbarHeight = "80px",
                DefaultBorderRadius = "6px",
            },
            Typography = new Typography
            {
                Default = new Default
                {
                    FontSize = ".825rem",
                    FontWeight = 400,
                    LineHeight = 1.43,
                    LetterSpacing = "normal",
                    FontFamily = new string[] { "Public Sans", "Roboto", "Arial", "sans-serif" }
                },
                H1 = new H1
                {
                    FontSize = "4rem",
                    FontWeight = 700,
                    LineHeight = 1.167,
                    LetterSpacing = "-.01562em"
                },
                H2 = new H2
                {
                    FontSize = "3.75rem",
                    FontWeight = 300,
                    LineHeight = 1.2,
                    LetterSpacing = "-.00833em"
                },
                H3 = new H3
                {
                    FontSize = "3rem",
                    FontWeight = 600,
                    LineHeight = 1.167,
                    LetterSpacing = "0"
                },
                H4 = new H4
                {
                    FontSize = "1.8rem",
                    FontWeight = 400,
                    LineHeight = 1.235,
                    LetterSpacing = ".00735em"
                },
                H5 = new H5
                {
                    FontSize = "1.5rem",
                    FontWeight = 400,
                    LineHeight = 1.334,
                    LetterSpacing = "0"
                },
                H6 = new H6
                {
                    FontSize = "1.125rem",
                    FontWeight = 600,
                    LineHeight = 1.6,
                    LetterSpacing = ".0075em"
                },
                Button = new Button
                {
                    FontSize = ".825rem",
                    FontWeight = 500,
                    LineHeight = 1.75,
                    LetterSpacing = ".02857em",
                    TextTransform = "none"


                },
                Subtitle1 = new Subtitle1
                {
                    FontSize = "1rem",
                    FontWeight = 400,
                    LineHeight = 1.75,
                    LetterSpacing = ".00938em"
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
                    FontSize = ".825rem",
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
                    LineHeight = 2.66,
                    LetterSpacing = ".08333em"
                }
            }
        };
        return theme;
    }
}
