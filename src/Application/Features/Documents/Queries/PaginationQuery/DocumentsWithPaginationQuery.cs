// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Documents.DTOs;
using CleanArchitecture.Blazor.Application.Features.Documents.Caching;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;

namespace CleanArchitecture.Blazor.Application.Features.Documents.Queries.PaginationQuery;

public class DocumentsWithPaginationQuery : PaginationFilter, IRequest<PaginatedData<DocumentDto>>, ICacheable
{
   
    public string TenantId { get; set; }
    public DocumentsWithPaginationQuery(string tenantId)
    {
        TenantId = tenantId;
    }
    public string CacheKey => $"{nameof(DocumentsWithPaginationQuery)},{this},{TenantId}";
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
        var userid = _currentUserService.UserId;
        var data = await _context.Documents
            .Specify(new DocumentsQuery(userid,request.TenantId,request.Keyword))
            .OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectTo<DocumentDto>(_mapper.ConfigurationProvider)
            .PaginatedDataAsync(request.PageNumber, request.PageSize);

        return data;
    }
    internal class DocumentsQuery : Specification<Document>
    {
        public DocumentsQuery(string userId,string tenantId,string? keyword)
        {
            this.Criteria = p => (p.CreatedBy == userId && p.IsPublic == false) || p.IsPublic == true;
            And(x => x.TenantId == tenantId);
            if (!string.IsNullOrEmpty(keyword))
            {
                And(x => x.Title!.Contains(keyword) || x.Description!.Contains(keyword));
            }
        }
    }
}
