using $safeprojectname$.Models.SideMenu;

namespace $safeprojectname$.Services.Navigation;

public interface IMenuService
{
    IEnumerable<MenuSectionModel> Features { get; }
}
