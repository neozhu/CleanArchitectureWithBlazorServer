// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.



using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Features.Loggers.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Loggers.Commands.Delete;

public class ClearLogsCommand : IRequest<Result>, ICacheInvalidator
{

    public string CacheKey => LogsCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => LogsCacheKey.SharedExpiryTokenSource();

}

public class ClearLogsCommandHandler :
             IRequestHandler<ClearLogsCommand, Result>

{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<ClearLogsCommandHandler> _localizer;
    public ClearLogsCommandHandler(
        IApplicationDbContext context,
        IStringLocalizer<ClearLogsCommandHandler> localizer,
         IMapper mapper
        )
    {
        _context = context;
        _localizer = localizer;
        _mapper = mapper;
    }
    public async Task<Result> Handle(ClearLogsCommand request, CancellationToken cancellationToken)
    {
        _context.Loggers.RemoveRange(_context.Loggers);
        await _context.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync();
    }

}

