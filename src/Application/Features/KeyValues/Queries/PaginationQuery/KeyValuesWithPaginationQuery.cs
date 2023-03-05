// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.KeyValues.DTOs;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Caching;
using CleanArchitecture.Blazor.Application.Features.AuditTrails.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.Queries.PaginationQuery;

public class KeyValuesWithPaginationQuery : PaginationFilterBase, ICacheableRequest<PaginatedData<KeyValueDto>>
{
    [CompareTo("Value", "Text", "Description")] // <-- This filter will be applied to Name or Brand or Description.
    [StringFilterOptions(StringFilterOption.Contains)]
    public string? Keyword { get; set; }
    [OperatorComparison(OperatorType.Equal)]
    public Picklist? Picklist { get; set; }
    public string CacheKey => $"{nameof(KeyValuesWithPaginationQuery)},{this}";

    public MemoryCacheEntryOptions? Options => KeyValueCacheKey.MemoryCacheEntryOptions;
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
#pragma warning disable CS8602
#pragma warning disable CS8604
    public async Task<PaginatedData<KeyValueDto>> Handle(KeyValuesWithPaginationQuery request, CancellationToken cancellationToken)
    {

        var data = await _context.KeyValues.ApplyFilterWithoutPagination(request)
             .ProjectTo<KeyValueDto>(_mapper.ConfigurationProvider)
             .PaginatedDataAsync(request.Page, request.PerPage);

        return data;
    }
}
