// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Razor.Application.Features.DocumentTypes.DTOs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using CleanArchitecture.Razor.Application.Features.DocumentTypes.Caching;

namespace CleanArchitecture.Razor.Application.Features.DocumentTypes.Queries.PaginationQuery;

public class DocumentTypesWithPaginationQuery : PaginationRequest, IRequest<PaginatedData<DocumentTypeDto>>, ICacheable
{
    public string CacheKey => $"{nameof(DocumentTypesWithPaginationQuery)},{this}";

    public MemoryCacheEntryOptions Options => DocumentTypeCacheKey.MemoryCacheEntryOptions;
}
public class DocumentTypesQueryHandler : IRequestHandler<DocumentTypesWithPaginationQuery, PaginatedData<DocumentTypeDto>>
{

    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DocumentTypesQueryHandler(

        IApplicationDbContext context,
        IMapper mapper
        )
    {

        _context = context;
        _mapper = mapper;
    }
    public async Task<PaginatedData<DocumentTypeDto>> Handle(DocumentTypesWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var filters = PredicateBuilder.FromFilter<DocumentType>(request.FilterRules);
        var data = await _context.DocumentTypes.Where(filters)
            .OrderBy($"{request.Sort} {request.Order}")
            .ProjectTo<DocumentTypeDto>(_mapper.ConfigurationProvider)
            .PaginatedDataAsync(request.Page, request.Rows);

        return data;
    }
}
