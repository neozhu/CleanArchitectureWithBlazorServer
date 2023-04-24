// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Documents.Caching;
using CleanArchitecture.Blazor.Application.Features.Documents.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Documents.Queries.PaginationQuery;

public class DocumentsWithPaginationQuery : PaginationFilter, ICacheableRequest<PaginatedData<DocumentDto>>
{
    public DocumentListView ListView { get; set; } = DocumentListView.All;
    public  UserProfile? CurrentUser { get; set; }
    public override string ToString()
    {
        return $"CurrentUserId:{CurrentUser?.UserId},ListView:{ListView},Search:{Keyword},OrderBy:{OrderBy} {SortDirection},{PageNumber},{PageSize}";
    }
    public string CacheKey => DocumentCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => DocumentCacheKey.MemoryCacheEntryOptions;

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
    public async Task<PaginatedData<DocumentDto>> Handle(DocumentsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Documents
            .Specify(new DocumentsQuery(request))
            .OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectTo<DocumentDto>(_mapper.ConfigurationProvider)
            .PaginatedDataAsync(request.PageNumber, request.PageSize);

        return data;
    }
    
    internal class DocumentsQuery : Specification<Document>
    {
        public DocumentsQuery(DocumentsWithPaginationQuery request)
        {
            Criteria = request.ListView switch
            {
                DocumentListView.All => p => (p.CreatedBy == request.CurrentUser.UserId && p.IsPublic == false) || (p.IsPublic == true && p.TenantId == request.CurrentUser.TenantId),
                DocumentListView.My => p => (p.CreatedBy == request.CurrentUser.UserId && p.TenantId == request.CurrentUser.TenantId),
                DocumentListView.CreatedToday => p => p.Created.Value.Date == DateTime.Now.Date,
                _ => throw new NotImplementedException()
            };



            if (!string.IsNullOrEmpty(request.Keyword))
            {
                And(x => x.Title.Contains(request.Keyword) || x.Description.Contains(request.Keyword) || x.Content.Contains(request.Keyword));
            }
        }
    }

    
}
public enum DocumentListView
{
    [Description("All")]
    All,
    [Description("My Document")]
    My,
    [Description("Created Toady")]
    CreatedToday,
}
