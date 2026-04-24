// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Mapster;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.Caching;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.DTOs;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.Queries.PaginationQuery;

public class PicklistSetsWithPaginationQuery : PicklistSetAdvancedFilter, ICacheableRequest<PaginatedData<PicklistSetDto>>
{
    public PicklistSetAdvancedSpecification Specification => new(this);
    public string CacheKey => $"{nameof(PicklistSetsWithPaginationQuery)},{this}";
    public IEnumerable<string>? Tags => PicklistSetCacheKey.Tags;

    public override string ToString()
    {
        return $"ListView:{ListView}-{Picklist}-{CurrentUser?.UserId},Search:{Keyword},OrderBy:{OrderBy} {SortDirection},{PageNumber},{PageSize}";
    }
}

public class PicklistSetsQueryHandler : IRequestHandler<PicklistSetsWithPaginationQuery, PaginatedData<PicklistSetDto>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly TypeAdapterConfig _typeAdapterConfig;

    public PicklistSetsQueryHandler(
        IApplicationDbContextFactory dbContextFactory,
        TypeAdapterConfig typeAdapterConfig
    )
    {
        _dbContextFactory = dbContextFactory;
        _typeAdapterConfig = typeAdapterConfig;
    }

    public async ValueTask<PaginatedData<PicklistSetDto>> Handle(PicklistSetsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        var data = await db.PicklistSets.OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<PicklistSet, PicklistSetDto>(request.Specification, request.PageNumber, request.PageSize, _typeAdapterConfig, cancellationToken);
        return data;
    }
}
