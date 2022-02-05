namespace Blazor.Server.UI.Models;

public class ThemeManagerModel
{
    public bool IsDarkMode { get; set; } = false;
    public string PrimaryColor { get; set; } = "#2d4275";
    public double BorderRadius { get; set; } = 4;
}