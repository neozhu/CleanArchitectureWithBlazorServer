

// Usage:
// This validator ensures that the DeleteOfferLineCommand is valid before attempting 
// to delete offerline records from the system. It verifies that the ID list is not 
// null and that all IDs are greater than zero.

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Commands.Delete;

public class DeleteOfferLineCommandValidator : AbstractValidator<DeleteOfferLineCommand>
{
        public DeleteOfferLineCommandValidator()
        {
          
            RuleFor(v => v.Id).NotNull().ForEach(v=>v.GreaterThan(0));
          
        }
}
    

