using CleanArchitecture.Blazor.Application.Features.PicklistSets.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.Mappers;
#pragma warning disable RMG020
#pragma warning disable RMG012
[Mapper]
public static partial class PicklistMapper
{
    public static partial PicklistSetDto ToDto(PicklistSet picklistSet);
    public static partial PicklistSet FromDto(PicklistSetDto dto);
    public static partial AddEditPicklistSetCommand ToEditCommand(PicklistSetDto dto);
    public static partial PicklistSet FromEditCommand(AddEditPicklistSetCommand command);
    public static partial void ApplyChangesFrom(AddEditPicklistSetCommand command, PicklistSet contact);
    public static partial IQueryable<PicklistSetDto> ProjectTo(this IQueryable<PicklistSet> q);
}
