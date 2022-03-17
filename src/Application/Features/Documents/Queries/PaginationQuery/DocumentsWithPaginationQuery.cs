// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Documents.DTOs;
using CleanArchitecture.Blazor.Application.Common.Specification;
using CleanArchitecture.Blazor.Application.Features.Documents.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Documents.Queries.PaginationQuery;

public class DocumentsWithPaginationQuery : PaginationFilter, IRequest<PaginatedData<DocumentDto>>, ICacheable
{
    public string CacheKey => $"{nameof(DocumentsWithPaginationQuery)},{this}";
    public MemoryCacheEntryOptions? Options => DocumentCacheKey.MemoryCacheEntryOptions;

}
public class DocumentsQueryHandler : IRequestHandler<DocumentsWithPaginationQuery, PaginatedData<DocumentDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DocumentsQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext context,
        IMapper mapper
        )
    {
        _currentUserService = currentUserService;
        _context = context;
        _mapper = mapper;
    }
    public async Task<PaginatedData<DocumentDto>> Handle(DocumentsWithPaginationQuery request, CancellationToken cancellationToken)
    {
    
        var data = await _context.Documents
            .Specify(new DocumentsQuery(await _currentUserService.UserId()))
            .Where(x=>x.Description.Contains(request.Keyword))
            .OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectTo<DocumentDto>(_mapper.ConfigurationProvider)
            .PaginatedDataAsync(request.PageNumber, request.PageSize);

        return data;
    }

    internal class DocumentsQuery : Specification<Document>
    {
        public DocumentsQuery(string userId)
        {
            this.AddInclude(x => x.DocumentType);
            this.Criteria = p => (p.CreatedBy == userId && p.IsPublic == false) || p.IsPublic == true;
        }
    }
}
