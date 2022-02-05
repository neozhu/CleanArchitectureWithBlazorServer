// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Razor.Application.Features.Customers.DTOs;

namespace CleanArchitecture.Razor.Application.Features.Customers.Queries.Export;

public class ExportCustomersQuery : IRequest<byte[]>
{
    public string filterRules { get; set; }
    public string sort { get; set; } = "Id";
    public string order { get; set; } = "desc";
}

public class ExportCustomersQueryHandler :
     IRequestHandler<ExportCustomersQuery, byte[]>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IExcelService _excelService;
    private readonly IStringLocalizer<ExportCustomersQueryHandler> _localizer;

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
    public async Task<byte[]> Handle(ExportCustomersQuery request, CancellationToken cancellationToken)
    {
        var filters = PredicateBuilder.FromFilter<Customer>(request.filterRules);
        var data = await _context.Customers.Where(filters)
            .OrderBy($"{request.sort} {request.order}")
            .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        var result = await _excelService.ExportAsync(data,
            new Dictionary<string, Func<CustomerDto, object>>()
            {
                    //{ _localizer["Id"], item => item.Id },
                    { _localizer["Name"], item => item.Name },
                    { _localizer["Name Of English"], item => item.NameOfEnglish },
                    { _localizer["Group Name"], item => item.GroupName },
                    { _localizer["Partner Type"], item => item.PartnerType },
                    { _localizer["Region"], item => item.Region },
                    { _localizer["Sales"], item => item.Sales },
                    { _localizer["Region Sales Director"], item => item.RegionSalesDirector },
                    { _localizer["Address"], item => item.Address },
                    { _localizer["Address Of English"], item => item.AddressOfEnglish },
                    { _localizer["Contact"], item => item.Contact },
                    { _localizer["Email"], item => item.Email },
                    { _localizer["Phone Number"], item => item.PhoneNumber },
                    { _localizer["Fax"], item => item.Fax },
                    { _localizer["Comments"], item => item.Comments },

            }, _localizer["Customers"]
            );
        return result;
    }


}
