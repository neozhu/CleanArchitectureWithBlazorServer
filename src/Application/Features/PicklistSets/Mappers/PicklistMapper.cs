using CleanArchitecture.Blazor.Application.Features.PicklistSets.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.DTOs;
using Riok.Mapperly.Abstractions;

namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.Mappers;
#pragma warning disable RMG020
#pragma warning disable RMG012
[Mapper]
public static partial class PicklistMapper
{
    public static partial PicklistSetDto ToDto(PicklistSet picklistSet);
    public static partial PicklistSet Map(PicklistSetDto dto);
    public static partial AddEditPicklistSetCommand ToEditCommand(PicklistSetDto dto);
    public static partial PicklistSet Map(AddEditPicklistSetCommand command);
    public static partial void MapTo(AddEditPicklistSetCommand command, PicklistSet contact);
    public static partial IQueryable<PicklistSetDto> ProjectTo(this IQueryable<PicklistSet> q);
}
