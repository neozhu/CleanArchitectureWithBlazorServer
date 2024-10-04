// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Contacts.DTOs;
using CleanArchitecture.Blazor.Application.Features.Contacts.Caching;
using CleanArchitecture.Blazor.Application.Features.Contacts.Specifications;

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
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<GetContactByIdQueryHandler> _localizer;

    public GetContactByIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IStringLocalizer<GetContactByIdQueryHandler> localizer
        )
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<Result<ContactDto>> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Contacts.ApplySpecification(new ContactByIdSpecification(request.Id))
                     .ProjectTo<ContactDto>(_mapper.ConfigurationProvider)
                     .FirstAsync(cancellationToken);
        return await Result<ContactDto>.SuccessAsync(data);
    }
}
