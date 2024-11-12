// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.PicklistSets.Caching;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.DTOs;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.Queries.ByName;

public class PicklistSetsQueryByName : ICacheableRequest<IEnumerable<PicklistSetDto>>
{
    public PicklistSetsQueryByName(Picklist name)
    {
        Name = name;
    }
    public Picklist Name { get; set; }
    public string CacheKey => PicklistSetCacheKey.GetCacheKey(Name.ToString());
    public IEnumerable<string>? Tags => PicklistSetCacheKey.Tags;
}

public class PicklistSetsQueryByNameHandler : IRequestHandler<PicklistSetsQueryByName, IEnumerable<PicklistSetDto>>
{
    private readonly IApplicationDbContext _context;
    public PicklistSetsQueryByNameHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PicklistSetDto>> Handle(PicklistSetsQueryByName request,
        CancellationToken cancellationToken)
    {
        var data = await _context.PicklistSets.Where(x => x.Name == request.Name)
            .OrderBy(x => x.Text)
            .ProjectTo()
            .ToListAsync(cancellationToken);
        return data;
    }
}