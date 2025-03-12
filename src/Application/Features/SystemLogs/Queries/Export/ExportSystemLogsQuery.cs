// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.SystemLogs.DTOs;
using CleanArchitecture.Blazor.Application.Features.SystemLogs.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.Loggers.Queries.Export;

public class ExportSystemLogsQuery : IRequest<byte[]>
{
    public string Keyword { get; set; } = string.Empty;
    public string OrderBy { get; set; } = "Id";
    public string SortDirection { get; set; } = "Descending";
}

public class ExportSystemLogsQueryHandler : IRequestHandler<ExportSystemLogsQuery, byte[]>
{
    private readonly IApplicationDbContext _context;
    private readonly IExcelService _excelService;
    private readonly IStringLocalizer<ExportSystemLogsQueryHandler> _localizer;

    public ExportSystemLogsQueryHandler(
        IApplicationDbContext context,
        IExcelService excelService,
        IStringLocalizer<ExportSystemLogsQueryHandler> localizer
    )
    {
        _context = context;
        _excelService = excelService;
        _localizer = localizer;
    }

    public async Task<byte[]> Handle(ExportSystemLogsQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.SystemLogs
            .Where(x => x.Message!.Contains(request.Keyword) || x.Exception!.Contains(request.Keyword))
            .OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectTo()
            .ToListAsync(cancellationToken);
        var result = await _excelService.ExportAsync(data,
            new Dictionary<string, Func<SystemLogDto, object?>>
            {
                //{ _localizer["Id"], item => item.Id },
                { _localizer["Time Stamp"], item => item.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss") },
                { _localizer["Level"], item => item.Level },
                { _localizer["Message"], item => item.Message },
                { _localizer["Exception"], item => item.Exception },
                { _localizer["User Name"], item => item.UserName },
                { _localizer["Message Template"], item => item.MessageTemplate },
                { _localizer["Properties"], item => item.Properties }
            }, _localizer["Logs"]
        );
        return result;
    }
}