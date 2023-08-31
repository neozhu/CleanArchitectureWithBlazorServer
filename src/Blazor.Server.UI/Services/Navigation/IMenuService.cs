using Blazor.Server.UI.Models.NavigationMenu;

namespace Blazor.Server.UI.Services.Navigation;

public interface IMenuService
{
    IEnumerable<MenuSectionModel> Features { get; }
}
