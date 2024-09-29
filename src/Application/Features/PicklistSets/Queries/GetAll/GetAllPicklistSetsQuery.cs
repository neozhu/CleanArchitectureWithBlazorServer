// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.PicklistSets.Caching;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.Queries.GetAll;

public class GetAllPicklistSetsQuery : ICacheableRequest<IEnumerable<PicklistSetDto>>
{
    public string CacheKey => PicklistSetCacheKey.GetAllCacheKey;

    public MemoryCacheEntryOptions? Options => PicklistSetCacheKey.MemoryCacheEntryOptions;
}

public class GetAllPicklistSetsQueryHandler : IRequestHandler<GetAllPicklistSetsQuery, IEnumerable<PicklistSetDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllPicklistSetsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PicklistSetDto>> Handle(GetAllPicklistSetsQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _context.PicklistSets.OrderBy(x => x.Name).ThenBy(x => x.Value)
            .ProjectTo<PicklistSetDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return data;
    }
}