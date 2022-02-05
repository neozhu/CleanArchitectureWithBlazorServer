// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Razor.Application.Features.KeyValues.Caching;
using CleanArchitecture.Razor.Application.Features.KeyValues.DTOs;
using Microsoft.Extensions.Caching.Memory;

namespace CleanArchitecture.Razor.Application.Features.KeyValues.Queries.ByName;

public class GetAllKeyValuesQuery : IRequest<IEnumerable<KeyValueDto>>, ICacheable
{

    public string CacheKey => KeyValueCacheKey.GetAllCacheKey;

    public MemoryCacheEntryOptions Options => KeyValueCacheKey.MemoryCacheEntryOptions;
}
public class GetAllKeyValuesQueryHandler : IRequestHandler<KeyValuesQueryByName, IEnumerable<KeyValueDto>>
{

    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllKeyValuesQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<KeyValueDto>> Handle(KeyValuesQueryByName request, CancellationToken cancellationToken)
    {
        var data = await _context.KeyValues
           .ProjectTo<KeyValueDto>(_mapper.ConfigurationProvider)
           .ToListAsync(cancellationToken);
        return data;
    }
}
