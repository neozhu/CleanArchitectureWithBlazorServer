namespace $safeprojectname$.Models.SideMenu;

public class MenuSectionModel
{
    public string Title { get; set; } = string.Empty;
    public string[]? Roles { get; set; }
    public List<MenuSectionItemModel>? SectionItems { get; set; }
}