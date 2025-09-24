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
    private readonly IApplicationDbContextFactory _dbContextFactory;

    public DeleteTenantCommandHandler(
        IApplicationDbContextFactory dbContextFactory
    )
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Result> Handle(DeleteTenantCommand request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        var items = await db.Tenants.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        db.Tenants.RemoveRange(items);
        await db.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync();
    }
}
