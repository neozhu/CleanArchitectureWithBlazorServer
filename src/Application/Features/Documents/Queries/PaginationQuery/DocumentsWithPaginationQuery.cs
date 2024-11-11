// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Documents.Caching;
using CleanArchitecture.Blazor.Application.Features.Documents.DTOs;
using CleanArchitecture.Blazor.Application.Features.Documents.Mappers;
using CleanArchitecture.Blazor.Application.Features.Documents.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.Documents.Queries.PaginationQuery;

public class DocumentsWithPaginationQuery : AdvancedDocumentsFilter, ICacheableRequest<PaginatedData<DocumentDto>>
{
    public AdvancedDocumentsSpecification Specification => new(this);

    public string CacheKey => DocumentCacheKey.GetPaginationCacheKey($"{this}");
    public IEnumerable<string>? Tags => DocumentCacheKey.Tags;

    public override string ToString()
    {
        return
            $"CurrentUserId:{CurrentUser?.UserId},ListView:{ListView},Search:{Keyword},OrderBy:{OrderBy} {SortDirection},{PageNumber},{PageSize}";
    }
}

public class DocumentsQueryHandler : IRequestHandler<DocumentsWithPaginationQuery, PaginatedData<DocumentDto>>
{
    private readonly IApplicationDbContext _context;

    public DocumentsQueryHandler(
        IApplicationDbContext context
    )
    {
        _context = context;
    }

    public async Task<PaginatedData<DocumentDto>> Handle(DocumentsWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _context.Documents.OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync(request.Specification, request.PageNumber,
                request.PageSize,DocumentMapper.ToDto, cancellationToken);

        return data;
    }
}