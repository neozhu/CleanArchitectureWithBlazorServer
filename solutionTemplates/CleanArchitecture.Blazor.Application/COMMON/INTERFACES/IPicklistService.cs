using CleanArchitecture.Blazor.$safeprojectname$.Features.KeyValues.DTOs;

namespace CleanArchitecture.Blazor.$safeprojectname$.Common.Interfaces;
public interface IPicklistService
{
    List<KeyValueDto> DataSource { get; } 
    event Action? OnChange;
    Task Initialize();
    Task Refresh();
}
