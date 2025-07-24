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
    private readonly IApplicationDbContextFactory _dbContextFactory;

    public ClearSystemLogsCommandHandler(
        IApplicationDbContextFactory dbContextFactory
    )
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Result> Handle(ClearSystemLogsCommand request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        await db.SystemLogs.ExecuteDeleteAsync(cancellationToken);
        return await Result.SuccessAsync();
    }
}