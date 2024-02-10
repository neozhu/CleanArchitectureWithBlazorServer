using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

public interface IUserService
{
    List<ApplicationUserDto> DataSource { get; }
    event Action? OnChange;
    void Initialize();
    void Refresh();
}