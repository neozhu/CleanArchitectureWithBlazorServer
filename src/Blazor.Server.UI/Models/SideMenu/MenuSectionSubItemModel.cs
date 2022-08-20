namespace Blazor.Server.UI.Models.SideMenu;

public class MenuSectionSubItemModel
{
    public string Title { get; set; }=string.Empty;
    public string? Href { get; set; }
    public string[]? Roles { get; set; }
    public PageStatus PageStatus { get; set; } = PageStatus.Completed;
    public string? Target { get; set; }
}