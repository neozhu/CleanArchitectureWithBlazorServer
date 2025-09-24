// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.PicklistSets.Caching;

namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.Commands.Delete;

public class DeletePicklistSetCommand : ICacheInvalidatorRequest<Result>
{
    public DeletePicklistSetCommand(int[] id)
    {
        Id = id;
    }

    public int[] Id { get; }
    public string CacheKey => PicklistSetCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => PicklistSetCacheKey.Tags;
}

public class DeletePicklistSetCommandHandler : IRequestHandler<DeletePicklistSetCommand, Result>

{
    private readonly IApplicationDbContextFactory _dbContextFactory;

    public DeletePicklistSetCommandHandler(
        IApplicationDbContextFactory dbContextFactory
    )
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Result> Handle(DeletePicklistSetCommand request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        var items = await db.PicklistSets.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            item.AddDomainEvent(new DeletedEvent<PicklistSet>(item));
        }
        db.PicklistSets.RemoveRange(items);
        await db.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync();
    }
}
