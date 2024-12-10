﻿
using CleanArchitecture.Blazor.Application.Features.Contacts.DTOs;
using CleanArchitecture.Blazor.Application.Features.Contacts.Mappers;
using CleanArchitecture.Blazor.Application.Features.Contacts.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Contacts.Queries.GetAll;

public class GetAllContactsQuery : ICacheableRequest<IEnumerable<ContactDto>>
{
   public string CacheKey => ContactCacheKey.GetAllCacheKey;
   public IEnumerable<string>? Tags => ContactCacheKey.Tags;
}

public class GetAllContactsQueryHandler :
     IRequestHandler<GetAllContactsQuery, IEnumerable<ContactDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllContactsQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ContactDto>> Handle(GetAllContactsQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Contacts.ProjectTo()
                                                .AsNoTracking()
                                                .ToListAsync(cancellationToken);
        return data;
    }
}


