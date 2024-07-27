// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Contacts.DTOs;
using CleanArchitecture.Blazor.Application.Features.Contacts.Caching;
using CleanArchitecture.Blazor.Application.Features.Contacts.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.Contacts.Queries.Pagination;

public class ContactsWithPaginationQuery : ContactAdvancedFilter, ICacheableRequest<PaginatedData<ContactDto>>
{
    public override string ToString()
    {
        return $"Listview:{ListView}, Search:{Keyword}, {OrderBy}, {SortDirection}, {PageNumber}, {PageSize}";
    }
    public string CacheKey => ContactCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => ContactCacheKey.MemoryCacheEntryOptions;
    public ContactAdvancedSpecification Specification => new ContactAdvancedSpecification(this);
}
    
public class ContactsWithPaginationQueryHandler :
         IRequestHandler<ContactsWithPaginationQuery, PaginatedData<ContactDto>>
{
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<ContactsWithPaginationQueryHandler> _localizer;

        public ContactsWithPaginationQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<ContactsWithPaginationQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<PaginatedData<ContactDto>> Handle(ContactsWithPaginationQuery request, CancellationToken cancellationToken)
        {
           var data = await _context.Contacts.OrderBy($"{request.OrderBy} {request.SortDirection}")
                                    .ProjectToPaginatedDataAsync<Contact, ContactDto>(request.Specification, request.PageNumber, request.PageSize, _mapper.ConfigurationProvider, cancellationToken);
            return data;
        }
}