// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Documents.Caching;
using CleanArchitecture.Blazor.Application.Features.Documents.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Documents.Queries.PaginationQuery;

public class DocumentsWithPaginationQuery : PaginationFilter, ICacheableRequest<PaginatedData<DocumentDto>>
{
    public DocumentListView ListView { get; set; } = DocumentListView.All;
    public required UserProfile CurrentUser { get; set; }
    public string CacheKey => DocumentCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => DocumentCacheKey.MemoryCacheEntryOptions;

    public override string ToString()
    {
        return
            $"CurrentUserId:{CurrentUser?.UserId},ListView:{ListView},Search:{Keyword},OrderBy:{OrderBy} {SortDirection},{PageNumber},{PageSize}";
    }

    public DocumentsQuerySpec Specification=>new DocumentsQuerySpec(this);
}

public class DocumentsQueryHandler : IRequestHandler<DocumentsWithPaginationQuery, PaginatedData<DocumentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DocumentsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedData<DocumentDto>> Handle(DocumentsWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _context.Documents.OrderBy($"{request.OrderBy} {request.SortDirection}")
                        .ProjectToPaginatedDataAsync<Document, DocumentDto>(request.Specification, request.PageNumber, request.PageSize, _mapper.ConfigurationProvider, cancellationToken);

        return data;
    }

    
}
public class DocumentsQuerySpec : Specification<Document>
{
    public DocumentsQuerySpec(DocumentsWithPaginationQuery request)
    {
        Query.Where(p =>
                (p.CreatedBy == request.CurrentUser.UserId && p.IsPublic == false) ||
                (p.IsPublic == true && p.TenantId == request.CurrentUser.TenantId), request.ListView == DocumentListView.All)
             .Where(p =>
                p.CreatedBy == request.CurrentUser.UserId && p.TenantId == request.CurrentUser.TenantId, request.ListView == DocumentListView.My)
             .Where(p => p.Created.Value.Date == DateTime.Now.Date, request.ListView == DocumentListView.CreatedToday)
             .Where(x => x.Title.Contains(request.Keyword) || x.Description.Contains(request.Keyword) || x.Content.Contains(request.Keyword), !string.IsNullOrEmpty(request.Keyword));

        }
}

public enum DocumentListView
{
    [Description("All")] All,
    [Description("My Document")] My,
    [Description("Created Toady")] CreatedToday
}