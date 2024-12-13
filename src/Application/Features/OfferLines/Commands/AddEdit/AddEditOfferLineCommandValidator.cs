
// Usage:
// This validator enforces constraints on the AddEditOfferLineCommand, such as
// maximum field length for ...

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Commands.AddEdit;

public class AddEditOfferLineCommandValidator : AbstractValidator<AddEditOfferLineCommand>
{
    public AddEditOfferLineCommandValidator()
    {
                RuleFor(v => v.OfferId).NotNull(); 
    RuleFor(v => v.ItemId).NotNull(); 
    RuleFor(v => v.Quantity).NotNull(); 
    RuleFor(v => v.Discount).NotNull(); 
    RuleFor(v => v.LineTotal).NotNull(); 

     }

}

