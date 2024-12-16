

using CleanArchitecture.Blazor.Application.Features.SupplyItems.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.SupplyItems.Commands.Create;
using CleanArchitecture.Blazor.Application.Features.SupplyItems.Commands.Update;
using CleanArchitecture.Blazor.Application.Features.SupplyItems.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.SupplyItems.Mappers;

#pragma warning disable RMG020
#pragma warning disable RMG012
[Mapper]
public static partial class SupplyItemMapper
{
    public static partial SupplyItemDto ToDto(SupplyItem source);
    public static partial SupplyItem FromDto(SupplyItemDto dto);
    public static partial SupplyItem FromEditCommand(AddEditSupplyItemCommand command);
    public static partial SupplyItem FromCreateCommand(CreateSupplyItemCommand command);
    public static partial UpdateSupplyItemCommand ToUpdateCommand(SupplyItemDto dto);
    public static partial AddEditSupplyItemCommand CloneFromDto(SupplyItemDto dto);
    public static partial void ApplyChangesFrom(UpdateSupplyItemCommand source, SupplyItem target);
    public static partial void ApplyChangesFrom(AddEditSupplyItemCommand source, SupplyItem target);
    public static partial IQueryable<SupplyItemDto> ProjectTo(this IQueryable<SupplyItem> q);
}

