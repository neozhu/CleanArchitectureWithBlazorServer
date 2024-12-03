
using CleanArchitecture.Blazor.Application.Features.Categories.Caching;
using CleanArchitecture.Blazor.Application.Features.Categories.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.Categories.Commands.Update;

public class UpdateCategoryCommand: ICacheInvalidatorRequest<Result<int>>
{
      [Description("Id")]
      public int Id { get; set; }
            [Description("Name")]
    public string Name {get;set;} 

      public string CacheKey => CategoryCacheKey.GetAllCacheKey;
      public IEnumerable<string>? Tags => CategoryCacheKey.Tags;

}

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    public UpdateCategoryCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<int>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {

       var item = await _context.Categories.FindAsync(request.Id, cancellationToken);
       if (item == null)
       {
           return await Result<int>.FailureAsync($"Category with id: [{request.Id}] not found.");
       }
       CategoryMapper.ApplyChangesFrom(request, item);
	    // raise a update domain event
	   item.AddDomainEvent(new CategoryUpdatedEvent(item));
       await _context.SaveChangesAsync(cancellationToken);
       return await Result<int>.SuccessAsync(item.Id);
    }
}

