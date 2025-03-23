﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This file is part of the CleanArchitecture.Blazor project.
//     Licensed to the .NET Foundation under the MIT license.
//     See the LICENSE file in the project root for more information.
//
//     Author: neozhu
//     Created Date: 2025-03-19
//     Last Modified: 2025-03-19
//     Description: 
//       Defines a query for retrieving contacts with pagination and filtering 
//       options. The result is cached to enhance performance for repeated queries.
// </auto-generated>
//------------------------------------------------------------------------------

using CleanArchitecture.Blazor.Application.Features.Contacts.DTOs;
using CleanArchitecture.Blazor.Application.Features.Contacts.Caching;
using CleanArchitecture.Blazor.Application.Features.Contacts.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.Contacts.Queries.Pagination;

public class ContactsWithPaginationQuery : ContactAdvancedFilter, ICacheableRequest<PaginatedData<ContactDto>>
{
    public override string ToString()
    {
        return $"Listview:{ListView}:{CurrentUser?.UserId}, Search:{Keyword}, {OrderBy}, {SortDirection}, {PageNumber}, {PageSize}";
    }
    public string CacheKey => ContactCacheKey.GetPaginationCacheKey($"{this}");
    public IEnumerable<string>? Tags => ContactCacheKey.Tags;
    public ContactAdvancedSpecification Specification => new ContactAdvancedSpecification(this);
}
    
public class ContactsWithPaginationQueryHandler :
         IRequestHandler<ContactsWithPaginationQuery, PaginatedData<ContactDto>>
{
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        public ContactsWithPaginationQueryHandler(
            IMapper mapper,
            IApplicationDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<PaginatedData<ContactDto>> Handle(ContactsWithPaginationQuery request, CancellationToken cancellationToken)
        {
           var data = await _context.Contacts.OrderBy($"{request.OrderBy} {request.SortDirection}")
                                                   .ProjectToPaginatedDataAsync<Contact, ContactDto>(request.Specification,
                                                    request.PageNumber,
                                                    request.PageSize,
                                                    _mapper.ConfigurationProvider,
                                                    cancellationToken);
            return data;
        }
}