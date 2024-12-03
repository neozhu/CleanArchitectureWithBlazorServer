

using CleanArchitecture.Blazor.Application.Features.Categories.DTOs;
using CleanArchitecture.Blazor.Application.Features.Categories.Caching;
using CleanArchitecture.Blazor.Application.Features.Categories.Mappers;
using CleanArchitecture.Blazor.Application.Features.Categories.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.Categories.Queries.GetById;

public class GetCategoryByIdQuery : ICacheableRequest<Result<CategoryDto>>
{
   public required int Id { get; set; }
   public string CacheKey => CategoryCacheKey.GetByIdCacheKey($"{Id}");
   public IEnumerable<string>? Tags => CategoryCacheKey.Tags;
}

public class GetCategoryByIdQueryHandler :
     IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCategoryByIdQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Categories.ApplySpecification(new CategoryByIdSpecification(request.Id))
                                                .ProjectTo()
                                                .FirstAsync(cancellationToken);
        return await Result<CategoryDto>.SuccessAsync(data);
    }
}
