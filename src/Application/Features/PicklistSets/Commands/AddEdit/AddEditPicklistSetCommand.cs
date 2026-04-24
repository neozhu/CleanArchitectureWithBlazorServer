// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.PicklistSets.Caching;
namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.Commands.AddEdit;

public class AddEditPicklistSetCommand : ICacheInvalidatorRequest<Result<int>>
{
    [Description("Id")] public int Id { get; set; }
    [Description("Name")] public Picklist Name { get; set; }
    [Description("Value")] public string? Value { get; set; }
    [Description("Text")] public string? Text { get; set; }
    [Description("Description")] public string? Description { get; set; }
    public TrackingState TrackingState { get; set; } = TrackingState.Unchanged;
    public string CacheKey => PicklistSetCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => PicklistSetCacheKey.Tags;}

public class AddEditPicklistSetCommandHandler : IRequestHandler<AddEditPicklistSetCommand, Result<int>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IObjectMapper _objectMapper;

    public AddEditPicklistSetCommandHandler(
        IApplicationDbContextFactory dbContextFactory,
        IObjectMapper objectMapper
    )
    {
        _dbContextFactory = dbContextFactory;
        _objectMapper = objectMapper;
    }

    public async ValueTask<Result<int>> Handle(AddEditPicklistSetCommand request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        if (request.Id > 0)
        {
            var item = await db.PicklistSets.FindAsync(request.Id, cancellationToken);
            if (item == null) return await Result<int>.FailureAsync($"PicklistSet with id: [{request.Id}] not found.");
            item = _objectMapper.Map(request, item);
            item.AddDomainEvent(new PicklistSetUpdatedEvent(item));
            await db.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }
        else
        {
            var keyValue = _objectMapper.Map<PicklistSet>(request);
            keyValue.AddDomainEvent(new PicklistSetCreatedEvent(keyValue));
            db.PicklistSets.Add(keyValue);
            await db.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(keyValue.Id);
        }
    }
}
