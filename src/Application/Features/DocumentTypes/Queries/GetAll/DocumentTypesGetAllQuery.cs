// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Razor.Application.Features.DocumentTypes.DTOs;
using Microsoft.Extensions.Caching.Memory;
using CleanArchitecture.Razor.Application.Features.DocumentTypes.Caching;

namespace CleanArchitecture.Razor.Application.Features.DocumentTypes.Queries.PaginationQuery;

public class DocumentTypesGetAllQuery : IRequest<IEnumerable<DocumentTypeDto>>, ICacheable
{

    public string CacheKey => DocumentTypeCacheKey.GetAllCacheKey;

    public MemoryCacheEntryOptions Options => null;
}
public class DocumentTypesGetAllQueryHandler : IRequestHandler<DocumentTypesGetAllQuery, IEnumerable<DocumentTypeDto>>
{

    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DocumentTypesGetAllQueryHandler(

        IApplicationDbContext context,
        IMapper mapper
        )
    {

        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<DocumentTypeDto>> Handle(DocumentTypesGetAllQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.DocumentTypes
            .OrderBy(x => x.Name)
            .ProjectTo<DocumentTypeDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return data;
    }
}
