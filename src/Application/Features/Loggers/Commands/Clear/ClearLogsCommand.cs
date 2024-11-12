// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.Loggers.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Loggers.Commands.Clear;

public class ClearLogsCommand : ICacheInvalidatorRequest<Result>
{
    public string CacheKey => LogsCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => LogsCacheKey.Tags;
}

public class ClearLogsCommandHandler : IRequestHandler<ClearLogsCommand, Result>

{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ClearLogsCommandHandler> _logger;

    public ClearLogsCommandHandler(
        IApplicationDbContext context,
        ILogger<ClearLogsCommandHandler> logger
    )
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result> Handle(ClearLogsCommand request, CancellationToken cancellationToken)
    {
        await _context.Loggers.ExecuteDeleteAsync();
        _logger.LogInformation("Logs have been erased");
        return await Result.SuccessAsync();
    }
}