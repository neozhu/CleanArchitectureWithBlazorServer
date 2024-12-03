

using CleanArchitecture.Blazor.Application.Features.Categories.Caching;


namespace CleanArchitecture.Blazor.Application.Features.Categories.Commands.Delete;

public class DeleteCategoryCommand:  ICacheInvalidatorRequest<Result<int>>
{
  public int[] Id {  get; }
  public string CacheKey => CategoryCacheKey.GetAllCacheKey;
  public IEnumerable<string>? Tags => CategoryCacheKey.Tags;
  public DeleteCategoryCommand(int[] id)
  {
    Id = id;
  }
}

public class DeleteCategoryCommandHandler : 
             IRequestHandler<DeleteCategoryCommand, Result<int>>

{
    private readonly IApplicationDbContext _context;
    public DeleteCategoryCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<int>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var items = await _context.Categories.Where(x=>request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        foreach (var item in items)
        {
		    // raise a delete domain event
			item.AddDomainEvent(new CategoryDeletedEvent(item));
            _context.Categories.Remove(item);
        }
        var result = await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(result);
    }

}

