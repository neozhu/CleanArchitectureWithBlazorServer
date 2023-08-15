// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Customers.DTOs;
using CleanArchitecture.Blazor.Application.Features.Customers.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.Customers.Queries.Export;

public class ExportCustomersQuery : CustomerAdvancedFilter, IRequest<Result<byte[]>>
{
      public CustomerAdvancedPaginationSpec Specification => new CustomerAdvancedPaginationSpec(this);
}
    
public class ExportCustomersQueryHandler :
         IRequestHandler<ExportCustomersQuery, Result<byte[]>>
{
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IExcelService _excelService;
        private readonly IStringLocalizer<ExportCustomersQueryHandler> _localizer;
        private readonly CustomerDto _dto = new();
        public ExportCustomersQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IExcelService excelService,
            IStringLocalizer<ExportCustomersQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _excelService = excelService;
            _localizer = localizer;
        }

        public async Task<Result<byte[]>> Handle(ExportCustomersQuery request, CancellationToken cancellationToken)
        {
            // TODO: Implement ExportCustomersQueryHandler method 
            var data = await _context.Customers.ApplySpecification(request.Specification)
                       .OrderBy($"{request.OrderBy} {request.SortDirection}")
                       .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
                       .AsNoTracking()
                       .ToListAsync(cancellationToken);
            var result = await _excelService.ExportAsync(data,
                new Dictionary<string, Func<CustomerDto, object?>>()
                {
                    // TODO: Define the fields that should be exported, for example:
                    {_localizer[_dto.GetMemberDescription(x=>x.Id)],item => item.Id}, 
{_localizer[_dto.GetMemberDescription(x=>x.Name)],item => item.Name}, 
{_localizer[_dto.GetMemberDescription(x=>x.Description)],item => item.Description}, 

                }
                , _localizer[_dto.GetClassDescription()]);
            return await Result<byte[]>.SuccessAsync(result);;
        }
}
#nullable disable warnings
public class CustomersExportSpecification : Specification<Customer>
{
    public CustomersExportSpecification(ExportCustomersQuery query)
    {
        var today = DateTime.Now.Date;
        var start = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00",
            CultureInfo.CurrentCulture);
        var end = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 23:59:59",
            CultureInfo.CurrentCulture);
        var last30day = Convert.ToDateTime(
            today.AddDays(-30).ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00",
            CultureInfo.CurrentCulture);

       Query.Where(q => q.Name != null)
             .Where(q => q.Name!.Contains(query.Keyword) || q.Description!.Contains(query.Keyword), !string.IsNullOrEmpty(query.Keyword))
             .Where(q => q.CreatedBy == query.CurrentUser.UserId, query.ListView == CustomerListView.My && query.CurrentUser is not null)
             .Where(q => q.Created >= start && q.Created <= end, query.ListView == CustomerListView.CreatedToday)
             .Where(q => q.Created >= last30day, query.ListView == CustomerListView.Created30Days);
       
    }
}
