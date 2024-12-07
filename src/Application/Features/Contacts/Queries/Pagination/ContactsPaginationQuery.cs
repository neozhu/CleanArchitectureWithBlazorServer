

using CleanArchitecture.Blazor.Application.Features.Contacts.DTOs;
using CleanArchitecture.Blazor.Application.Features.Contacts.Caching;
using CleanArchitecture.Blazor.Application.Features.Contacts.Mappers;
using CleanArchitecture.Blazor.Application.Features.Contacts.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.Contacts.Queries.Pagination;

public class ContactsWithPaginationQuery : ContactAdvancedFilter, ICacheableRequest<PaginatedData<ContactDto>>
{
    public override string ToString()
    {
        return $"Listview:{ListView}:{CurrentUser?.UserId}-{LocalTimezoneOffset.TotalHours}, Search:{Keyword}, {OrderBy}, {SortDirection}, {PageNumber}, {PageSize}";
    }
    public string CacheKey => ContactCacheKey.GetPaginationCacheKey($"{this}");
    public IEnumerable<string>? Tags => ContactCacheKey.Tags;
    public ContactAdvancedSpecification Specification => new ContactAdvancedSpecification(this);
}
    
public class ContactsWithPaginationQueryHandler :
         IRequestHandler<ContactsWithPaginationQuery, PaginatedData<ContactDto>>
{
        private readonly IApplicationDbContext _context;

        public ContactsWithPaginationQueryHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedData<ContactDto>> Handle(ContactsWithPaginationQuery request, CancellationToken cancellationToken)
        {
           var data = await _context.Contacts.OrderBy($"{request.OrderBy} {request.SortDirection}")
                                                   .ProjectToPaginatedDataAsync(request.Specification, 
                                                                                request.PageNumber, 
                                                                                request.PageSize, 
                                                                                ContactMapper.ToDto, 
                                                                                cancellationToken);
            return data;
        }
}