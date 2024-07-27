// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Contacts.DTOs;
using CleanArchitecture.Blazor.Application.Features.Contacts.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Contacts.Queries.GetAll;

public class GetAllContactsQuery : ICacheableRequest<IEnumerable<ContactDto>>
{
   public string CacheKey => ContactCacheKey.GetAllCacheKey;
   public MemoryCacheEntryOptions? Options => ContactCacheKey.MemoryCacheEntryOptions;
}

public class GetAllContactsQueryHandler :
     IRequestHandler<GetAllContactsQuery, IEnumerable<ContactDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<GetAllContactsQueryHandler> _localizer;

    public GetAllContactsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IStringLocalizer<GetAllContactsQueryHandler> localizer
        )
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<IEnumerable<ContactDto>> Handle(GetAllContactsQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Contacts
                     .ProjectTo<ContactDto>(_mapper.ConfigurationProvider)
                     .AsNoTracking()
                     .ToListAsync(cancellationToken);
        return data;
    }
}


