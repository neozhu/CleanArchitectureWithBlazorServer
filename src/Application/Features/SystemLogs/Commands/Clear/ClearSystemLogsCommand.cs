// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.SystemLogs.Caching;

namespace CleanArchitecture.Blazor.Application.Features.SystemLogs.Commands.Clear;

public class ClearSystemLogsCommand : ICacheInvalidatorRequest<Result>
{
    public string CacheKey => SystemLogsCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => SystemLogsCacheKey.Tags;
}

public class ClearSystemLogsCommandHandler : IRequestHandler<ClearSystemLogsCommand, Result>

{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ClearSystemLogsCommandHandler> _logger;

    public ClearSystemLogsCommandHandler(
        IApplicationDbContext context,
        ILogger<ClearSystemLogsCommandHandler> logger
    )
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result> Handle(ClearSystemLogsCommand request, CancellationToken cancellationToken)
    {
        await _context.SystemLogs.ExecuteDeleteAsync();
        _logger.LogInformation("Logs have been erased");
        return await Result.SuccessAsync();
    }
}