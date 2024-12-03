
using CleanArchitecture.Blazor.Application.Features.Categories.DTOs;
using CleanArchitecture.Blazor.Application.Features.Categories.Mappers;
using CleanArchitecture.Blazor.Application.Features.Categories.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Categories.Queries.GetAll;

public class GetAllCategoriesQuery : ICacheableRequest<IEnumerable<CategoryDto>>
{
   public string CacheKey => CategoryCacheKey.GetAllCacheKey;
   public IEnumerable<string>? Tags => CategoryCacheKey.Tags;
}

public class GetAllCategoriesQueryHandler :
     IRequestHandler<GetAllCategoriesQuery, IEnumerable<CategoryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllCategoriesQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Categories.ProjectTo()
                                                .AsNoTracking()
                                                .ToListAsync(cancellationToken);
        return data;
    }
}


