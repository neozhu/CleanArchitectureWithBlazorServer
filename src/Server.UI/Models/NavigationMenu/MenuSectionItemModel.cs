namespace CleanArchitecture.Blazor.Server.UI.Models.NavigationMenu;

public class MenuSectionItemModel
{
    public string Title { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Href { get; set; }
    public string? Target { get; set; }
    public string[]? Roles { get; set; }
    public PageStatus PageStatus { get; set; } = PageStatus.Completed;
    public bool IsParent { get; set; }
    public List<MenuSectionSubItemModel>? MenuItems { get; set; }
}