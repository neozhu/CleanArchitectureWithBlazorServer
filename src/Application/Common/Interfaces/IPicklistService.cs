using CleanArchitecture.Blazor.Application.Features.PicklistSets.DTOs;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

public interface IPicklistService
{
    List<PicklistSetDto> DataSource { get; }
    event Func<Task>? OnChange;
    void Initialize();
    void Refresh();
}