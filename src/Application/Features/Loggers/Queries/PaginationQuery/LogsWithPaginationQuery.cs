// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Logs.DTOs;
using CleanArchitecture.Blazor.Domain.Entities.Log;

namespace CleanArchitecture.Blazor.Application.Logs.Queries.PaginationQuery;

public class LogsWithPaginationQuery : PaginationFilter, IRequest<PaginatedData<LogDto>>
{


}
public class LogsQueryHandler : IRequestHandler<LogsWithPaginationQuery, PaginatedData<LogDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public LogsQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext context,
        IMapper mapper
        )
    {
        _currentUserService = currentUserService;
        _context = context;
        _mapper = mapper;
    }
    public async Task<PaginatedData<LogDto>> Handle(LogsWithPaginationQuery request, CancellationToken cancellationToken)
    {
   

        var data = await _context.Loggers
            .Where(x=>x.Message.Contains(request.Keyword) || x.Exception.Contains(request.Keyword))
            .OrderBy($"{request.OrderBy} {request.SortDirection}")
                .ProjectTo<LogDto>(_mapper.ConfigurationProvider)
                .PaginatedDataAsync(request.PageNumber, request.PageSize);

        return data;
    }


}
