


using CleanArchitecture.Blazor.Application.Features.Offers.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.Offers.Commands.Create;
using CleanArchitecture.Blazor.Application.Features.Offers.Commands.Update;
using CleanArchitecture.Blazor.Application.Features.Offers.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Offers.Mappers;

#pragma warning disable RMG020
#pragma warning disable RMG012
[Mapper]
public static partial class OfferMapper
{
    public static partial OfferDto ToDto(Offer source);
    public static partial Offer FromDto(OfferDto dto);
    public static partial Offer FromEditCommand(AddEditOfferCommand command);
    public static partial Offer FromCreateCommand(CreateOfferCommand command);
    public static partial UpdateOfferCommand ToUpdateCommand(OfferDto dto);
    public static partial AddEditOfferCommand ToEditCommand(OfferDto dto);
    public static partial AddEditOfferCommand CloneFromDto(OfferDto dto);
    public static partial void ApplyChangesFrom(UpdateOfferCommand source, Offer target);
    public static partial void ApplyChangesFrom(AddEditOfferCommand source, Offer target);
    public static partial IQueryable<OfferDto> ProjectTo(this IQueryable<Offer> q);
}

