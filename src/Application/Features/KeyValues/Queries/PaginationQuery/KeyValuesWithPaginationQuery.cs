// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.KeyValues.Caching;
using CleanArchitecture.Blazor.Application.Features.KeyValues.DTOs;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.Queries.PaginationQuery;

public class KeyValuesWithPaginationQuery : KeyValueAdvancedFilter, ICacheableRequest<PaginatedData<KeyValueDto>>
{
    public KeyValueAdvancedSpecification Specification => new(this);

    public string CacheKey => $"{nameof(KeyValuesWithPaginationQuery)},{this}";
    public MemoryCacheEntryOptions? Options => KeyValueCacheKey.MemoryCacheEntryOptions;

    public override string ToString()
    {
        return $"Picklist:{Picklist},Search:{Keyword},OrderBy:{OrderBy} {SortDirection},{PageNumber},{PageSize}";
    }
}

public class KeyValuesQueryHandler : IRequestHandler<KeyValuesWithPaginationQuery, PaginatedData<KeyValueDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public KeyValuesQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedData<KeyValueDto>> Handle(KeyValuesWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _context.KeyValues.OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<KeyValue, KeyValueDto>(request.Specification, request.PageNumber,
                request.PageSize, _mapper.ConfigurationProvider, cancellationToken);

        return data;
    }
}