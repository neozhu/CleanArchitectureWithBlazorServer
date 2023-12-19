using CleanArchitecture.Blazor.Server.UI.Models.NavigationMenu;

namespace CleanArchitecture.Blazor.Server.UI.Services.Navigation;

public interface IMenuService
{
    IEnumerable<MenuSectionModel> Features { get; }
}