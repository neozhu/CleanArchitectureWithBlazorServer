// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.AuditTrails.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.AuditTrails.Queries.Export;

public class ExportAuditTrailsQuery : IRequest<byte[]>
{
    public string filterRules { get; set; }
    public string sort { get; set; } = "Id";
    public string order { get; set; } = "desc";
}

public class ExportAuditTrailsQueryHandler :
     IRequestHandler<ExportAuditTrailsQuery, byte[]>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IExcelService _excelService;
    private readonly IStringLocalizer<ExportAuditTrailsQueryHandler> _localizer;

    public ExportAuditTrailsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IExcelService excelService,
        IStringLocalizer<ExportAuditTrailsQueryHandler> localizer
        )
    {
        _context = context;
        _mapper = mapper;
        _excelService = excelService;
        _localizer = localizer;
    }

    public async Task<byte[]> Handle(ExportAuditTrailsQuery request, CancellationToken cancellationToken)
    {
        var filters = PredicateBuilder.FromFilter<AuditTrail>(request.filterRules);
        var data = await _context.AuditTrails
            .Where(filters)
            .OrderBy($"{request.sort} {request.order}")
            .ProjectTo<AuditTrailDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        var result = await _excelService.ExportAsync(data,
            new Dictionary<string, Func<AuditTrailDto, object>>()
            {
                    //{ _localizer["Id"], item => item.Id },
                    { _localizer["Date Time"], item => item.DateTime.ToString("yyyy-MM-dd HH:mm:ss") },
                    { _localizer["Table Name"], item => item.TableName },
                    { _localizer["Audit Type"], item => item.AuditType },
                    { _localizer["Old Values"], item => item.OldValues },
                    { _localizer["New Values"], item => item.NewValues },
                    { _localizer["Primary Key"], item => item.PrimaryKey },
            }, _localizer["AuditTrails"]
            );
        return result;
    }
}

