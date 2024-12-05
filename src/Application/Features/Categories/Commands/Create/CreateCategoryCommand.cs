
using CleanArchitecture.Blazor.Application.Features.Categories.Caching;
using CleanArchitecture.Blazor.Application.Features.Categories.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.Categories.Commands.Create;

public class CreateCategoryCommand: ICacheInvalidatorRequest<Result<int>>
{
      [Description("Id")]
      public int Id { get; set; }
          [Description("Name")]
    public string Name {get;set;}
    [Description("Comments")]
    public string Comments { get; set; }
    public string CacheKey => CategoryCacheKey.GetAllCacheKey;
      public IEnumerable<string>? Tags => CategoryCacheKey.Tags;
}
    
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        public CreateCategoryCommandHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<int>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
           var item = CategoryMapper.FromCreateCommand(request);
           // raise a create domain event
	       item.AddDomainEvent(new CategoryCreatedEvent(item));
           _context.Categories.Add(item);
           await _context.SaveChangesAsync(cancellationToken);
           return  await Result<int>.SuccessAsync(item.Id);
        }
    }

