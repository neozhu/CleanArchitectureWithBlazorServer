// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Application.Features.Tenants.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Tenants.Commands.Delete;

public class DeleteTenantCommand : ICacheInvalidatorRequest<Result>
{
    public DeleteTenantCommand(string[] id)
    {
        Id = id;
    }
    public string[] Id { get; }
    public string CacheKey => TenantCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => TenantCacheKey.Tags;
}

public class DeleteTenantCommandHandler :
    IRequestHandler<DeleteTenantCommand, Result>

{
    private readonly IApplicationDbContext _context;
    private readonly ITenantService _tenantsService;

    public DeleteTenantCommandHandler(
        ITenantService tenantsService,
        IApplicationDbContext context
    )
    {
        _tenantsService = tenantsService;
        _context = context;
    }

    public async Task<Result> Handle(DeleteTenantCommand request, CancellationToken cancellationToken)
    {
        var items = await _context.Tenants.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        foreach (var item in items) _context.Tenants.Remove(item);

        await _context.SaveChangesAsync(cancellationToken);
        _tenantsService.Refresh();
        return await Result.SuccessAsync();
    }
}