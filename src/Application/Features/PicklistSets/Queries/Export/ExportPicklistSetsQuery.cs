// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.PicklistSets.DTOs;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.Mappers;

namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.Queries.Export;

public class ExportPicklistSetsQuery : IRequest<byte[]>
{
    public string? Keyword { get; set; }
    public string OrderBy { get; set; } = "Id";
    public string SortDirection { get; set; } = "desc";
}

public class ExportPicklistSetsQueryHandler :
    IRequestHandler<ExportPicklistSetsQuery, byte[]>
{
    private readonly IApplicationDbContext _context;
    private readonly IExcelService _excelService;
    private readonly IStringLocalizer<ExportPicklistSetsQueryHandler> _localizer;

    public ExportPicklistSetsQueryHandler(
        IApplicationDbContext context,
        IExcelService excelService,
        IStringLocalizer<ExportPicklistSetsQueryHandler> localizer
    )
    {
        _context = context;
        _excelService = excelService;
        _localizer = localizer;
    }
#pragma warning disable CS8602
#pragma warning disable CS8604
    public async Task<byte[]> Handle(ExportPicklistSetsQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.PicklistSets.Where(x =>
                x.Description.Contains(request.Keyword) || x.Value.Contains(request.Keyword) ||
                x.Text.Contains(request.Keyword))
            .OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectTo()
            .ToListAsync(cancellationToken);
        var result = await _excelService.ExportAsync(data,
            new Dictionary<string, Func<PicklistSetDto, object?>>
            {
                //{ _localizer["Id"], item => item.Id },
                { _localizer["Name"], item => item.Name },
                { _localizer["Value"], item => item.Value },
                { _localizer["Text"], item => item.Text },
                { _localizer["Description"], item => item.Description }
            }, _localizer["Data"]
        );
        return result;
    }
}