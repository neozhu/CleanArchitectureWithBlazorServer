// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Mapster;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.Caching;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.DTOs;


namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.Queries.GetAll;

public class GetAllPicklistSetsQuery : ICacheableRequest<IEnumerable<PicklistSetDto>>
{
    public string CacheKey => PicklistSetCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => PicklistSetCacheKey.Tags;
}

public class GetAllPicklistSetsQueryHandler : IRequestHandler<GetAllPicklistSetsQuery, IEnumerable<PicklistSetDto>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly TypeAdapterConfig _typeAdapterConfig;

    public GetAllPicklistSetsQueryHandler(
        IApplicationDbContextFactory dbContextFactory,
        TypeAdapterConfig typeAdapterConfig
    )
    {
        _dbContextFactory = dbContextFactory;
        _typeAdapterConfig = typeAdapterConfig;
    }

    public async ValueTask<IEnumerable<PicklistSetDto>> Handle(GetAllPicklistSetsQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        var data = await db.PicklistSets.OrderBy(x => x.Name).ThenBy(x => x.Value)
            .ProjectToType<PicklistSetDto>(_typeAdapterConfig)
            .ToListAsync(cancellationToken);
        return data;
    }
}
