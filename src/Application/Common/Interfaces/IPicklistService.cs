using CleanArchitecture.Blazor.Application.Features.KeyValues.DTOs;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

public interface IPicklistService
{
    List<KeyValueDto> DataSource { get; }
    event Func<Task>? OnChange;
    void Initialize();
    void Refresh();
}