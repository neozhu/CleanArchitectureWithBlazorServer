// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Razor.Application.Features.KeyValues.DTOs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using CleanArchitecture.Razor.Application.Features.KeyValues.Caching;

namespace CleanArchitecture.Razor.Application.Features.KeyValues.Queries.PaginationQuery;

public class KeyValuesWithPaginationQuery : PaginationRequest, IRequest<PaginatedData<KeyValueDto>>, ICacheable
{
    public string CacheKey => $"KeyValuesWithPaginationQuery,{this.ToString()}";

    public MemoryCacheEntryOptions Options => KeyValueCacheKey.MemoryCacheEntryOptions;
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
    public async Task<PaginatedData<KeyValueDto>> Handle(KeyValuesWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var filters = PredicateBuilder.FromFilter<KeyValue>(request.FilterRules);
        var data = await _context.KeyValues.Where(filters)
            .OrderBy($"{request.Sort} {request.Order}")
            .ProjectTo<KeyValueDto>(_mapper.ConfigurationProvider)
            .PaginatedDataAsync(request.Page, request.Rows);

        return data;
    }
}
