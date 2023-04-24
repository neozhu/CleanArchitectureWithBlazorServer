

using CleanArchitecture.Blazor.Application.Features.Identity.Dto;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
public interface IUserDataProvider
{
    List<ApplicationUserDto> DataSource { get; }
    event Action? OnChange;
    Task InitializeAsync();
    void Initialize();
    Task Refresh();
}
