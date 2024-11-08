// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Contacts.DTOs;
using CleanArchitecture.Blazor.Application.Features.Contacts.Caching;
using CleanArchitecture.Blazor.Application.Features.Contacts.Specifications;
using CleanArchitecture.Blazor.Application.Features.Contacts.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.Contacts.Queries.GetById;

public class GetContactByIdQuery : ICacheableRequest<Result<ContactDto>>
{
   public required int Id { get; set; }
   public string CacheKey => ContactCacheKey.GetByIdCacheKey($"{Id}");
   public MemoryCacheEntryOptions? Options => ContactCacheKey.MemoryCacheEntryOptions;
}

public class GetContactByIdQueryHandler :
     IRequestHandler<GetContactByIdQuery, Result<ContactDto>>
{
    private readonly IApplicationDbContext _context;

    public GetContactByIdQueryHandler(
        IApplicationDbContext context
        )
    {
        _context = context;
    }

    public async Task<Result<ContactDto>> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Contacts.ApplySpecification(new ContactByIdSpecification(request.Id))
                     .ProjectTo()
                     .FirstAsync(cancellationToken);
        return await Result<ContactDto>.SuccessAsync(data);
    }
}
