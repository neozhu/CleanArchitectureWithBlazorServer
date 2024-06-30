// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Application.Features.Tenants.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Tenants.Commands.Delete;

public class DeleteTenantCommand : ICacheInvalidatorRequest<Result<int>>
{
    public DeleteTenantCommand(string[] id)
    {
        Id = id;
    }

    public string[] Id { get; }
    public string CacheKey => TenantCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => TenantCacheKey.GetOrCreateTokenSource();
}

public class DeleteTenantCommandHandler :
    IRequestHandler<DeleteTenantCommand, Result<int>>

{
    private readonly IApplicationDbContext _context;
    private readonly IStringLocalizer<DeleteTenantCommandHandler> _localizer;
    private readonly IMapper _mapper;
    private readonly ITenantService _tenantsService;

    public DeleteTenantCommandHandler(
        ITenantService tenantsService,
        IApplicationDbContext context,
        IStringLocalizer<DeleteTenantCommandHandler> localizer,
        IMapper mapper
    )
    {
        _tenantsService = tenantsService;
        _context = context;
        _localizer = localizer;
        _mapper = mapper;
    }

    public async Task<Result<int>> Handle(DeleteTenantCommand request, CancellationToken cancellationToken)
    {
        var items = await _context.Tenants.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        foreach (var item in items) _context.Tenants.Remove(item);

        var result = await _context.SaveChangesAsync(cancellationToken);
        _tenantsService.Refresh();
        return await Result<int>.SuccessAsync(result);
    }
}