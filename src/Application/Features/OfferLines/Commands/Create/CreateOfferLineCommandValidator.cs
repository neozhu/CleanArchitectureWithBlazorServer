
// Usage:
// This validator is used to ensure that a CreateOfferLineCommand meets the required
// validation criteria. It enforces constraints like maximum field lengths and 
// ensures that the Name field is not empty before proceeding with the command execution.

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Commands.Create;

public class CreateOfferLineCommandValidator : AbstractValidator<CreateOfferLineCommand>
{
        public CreateOfferLineCommandValidator()
        {
                RuleFor(v => v.OfferId).NotNull(); 
    RuleFor(v => v.ItemId).NotNull(); 
    RuleFor(v => v.Quantity).NotNull(); 
    RuleFor(v => v.Discount).NotNull(); 
    RuleFor(v => v.LineTotal).NotNull(); 

        }
       
}

