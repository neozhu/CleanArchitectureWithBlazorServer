// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Logs.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Logs.Queries.Export;

public class ExportLogsQuery : IRequest<byte[]>
{
    public string filterRules { get; set; }
    public string sort { get; set; } = "Id";
    public string order { get; set; } = "desc";
}

public class ExportLogsQueryHandler :
     IRequestHandler<ExportLogsQuery, byte[]>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IExcelService _excelService;
    private readonly IStringLocalizer<ExportLogsQueryHandler> _localizer;

    public ExportLogsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IExcelService excelService,
        IStringLocalizer<ExportLogsQueryHandler> localizer
        )
    {
        _context = context;
        _mapper = mapper;
        _excelService = excelService;
        _localizer = localizer;
    }

    public async Task<byte[]> Handle(ExportLogsQuery request, CancellationToken cancellationToken)
    {
        var filters = PredicateBuilder.FromFilter<Logger>(request.filterRules);
        var data = await _context.Loggers
            .Where(filters)
            .OrderBy($"{request.sort} {request.order}")
            .ProjectTo<LogDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        var result = await _excelService.ExportAsync(data,
            new Dictionary<string, Func<LogDto, object>>()
            {
                    //{ _localizer["Id"], item => item.Id },
                    { _localizer["Time Stamp"], item => item.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss") },
                    { _localizer["Level"], item => item.Level },
                    { _localizer["Message"], item => item.Message },
                    { _localizer["Exception"], item => item.Exception },
                    { _localizer["User Name"], item => item.UserName },
                    { _localizer["Message Template"], item => item.MessageTemplate },
                    { _localizer["Properties"], item => item.Properties },
            }, _localizer["Logs"]
            );
        return result;
    }
}

