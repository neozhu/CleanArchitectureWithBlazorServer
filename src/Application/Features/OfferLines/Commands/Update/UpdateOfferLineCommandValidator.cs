

// Usage:
// The `UpdateOfferLineCommandValidator` is used to validate that an `UpdateOfferLineCommand` 
// contains valid and complete data before processing the update. It enforces rules such as 
// ensuring that the `Id` is provided, the `Name` is not empty, and certain properties 
// (e.g., ...) do not exceed their maximum 
// allowed length.

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Commands.Update;

public class UpdateOfferLineCommandValidator : AbstractValidator<UpdateOfferLineCommand>
{
        public UpdateOfferLineCommandValidator()
        {
           RuleFor(v => v.Id).NotNull();
               RuleFor(v => v.OfferId).NotNull(); 
    RuleFor(v => v.ItemId).NotNull(); 
    RuleFor(v => v.Quantity).NotNull(); 
    RuleFor(v => v.Discount).NotNull(); 
    RuleFor(v => v.LineTotal).NotNull(); 

          
        }
    
}

