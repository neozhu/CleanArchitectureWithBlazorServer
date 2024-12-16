
using CleanArchitecture.Blazor.Application.Features.OfferLines.Caching;

namespace CleanArchitecture.Blazor.Application.Features.OfferLines.Commands.Delete;

public class DeleteOfferLineCommand : ICacheInvalidatorRequest<Result<int>>
{
    public required int OfferId { get; set; }
    public required int[] Id { get; set; }
    public string CacheKey => OfferLineCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => OfferLineCacheKey.Tags;
}

public class DeleteOfferLineCommandHandler :
             IRequestHandler<DeleteOfferLineCommand, Result<int>>

{
    private readonly IApplicationDbContext _context;
    public DeleteOfferLineCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<int>> Handle(DeleteOfferLineCommand request, CancellationToken cancellationToken)
    {
        var offer = await _context.Offers
          .Include(o => o.OfferLines) 
          .FirstOrDefaultAsync(o => o.Id == request.OfferId, cancellationToken);

        if (offer is null)
            return await Result<int>.FailureAsync($"Offer with id: [{request.OfferId}] not found.");

        var linesToDelete = offer.OfferLines
            .Where(x => request.Id.Contains(x.Id))
            .ToList();

        if (!linesToDelete.Any())
            return await Result<int>.FailureAsync("No matching OfferLines found to delete.");

        linesToDelete.ForEach(line => offer.OfferLines.Remove(line));

        var result = await _context.SaveChangesAsync(cancellationToken);

        return await Result<int>.SuccessAsync(result);
    }

}

