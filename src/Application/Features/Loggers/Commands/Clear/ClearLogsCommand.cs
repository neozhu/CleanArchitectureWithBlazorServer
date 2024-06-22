// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.Loggers.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Loggers.Commands.Clear;

public class ClearLogsCommand : ICacheInvalidatorRequest<Result>
{
    public string CacheKey => LogsCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => LogsCacheKey.GetOrCreateTokenSource();
}

public class ClearLogsCommandHandler : IRequestHandler<ClearLogsCommand, Result>

{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<ClearLogsCommandHandler> _localizer;
    private readonly ILogger<ClearLogsCommandHandler> _logger;
    private readonly IMapper _mapper;

    public ClearLogsCommandHandler(
        IApplicationDbContext context,
        IStringLocalizer<ClearLogsCommandHandler> localizer,
        ILogger<ClearLogsCommandHandler> logger,
        IMapper mapper,
        ICurrentUserService currentUserService
    )
    {
        _context = context;
        _localizer = localizer;
        _logger = logger;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(ClearLogsCommand request, CancellationToken cancellationToken)
    {
        _context.Loggers.RemoveRange(_context.Loggers);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Logs have been erased by {@UserName:l}", _currentUserService.UserName);
        return await Result.SuccessAsync();
    }
}