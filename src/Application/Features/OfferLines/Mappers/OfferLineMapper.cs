

using CleanArchitecture.Blazor.Application.Features.OfferLines.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Commands.Create;
using CleanArchitecture.Blazor.Application.Features.OfferLines.Commands.Update;
using CleanArchitecture.Blazor.Application.Features.OfferLines.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Mappers;

#pragma warning disable RMG020
#pragma warning disable RMG012
[Mapper]
public static partial class OfferLineMapper
{
    public static partial OfferLineDto ToDto(OfferLine source);
    public static partial OfferLine FromDto(OfferLineDto dto);
    public static partial OfferLine FromEditCommand(AddEditOfferLineCommand command);
    public static partial OfferLine FromCreateCommand(CreateOfferLineCommand command);
    public static partial UpdateOfferLineCommand ToUpdateCommand(OfferLineDto dto);
    public static partial AddEditOfferLineCommand CloneFromDto(OfferLineDto dto);
    public static partial void ApplyChangesFrom(UpdateOfferLineCommand source, OfferLine target);
    public static partial void ApplyChangesFrom(AddEditOfferLineCommand source, OfferLine target);
    public static partial IQueryable<OfferLineDto> ProjectTo(this IQueryable<OfferLine> q);
}

