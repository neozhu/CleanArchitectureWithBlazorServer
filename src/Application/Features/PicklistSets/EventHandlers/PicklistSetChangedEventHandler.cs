// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.PicklistSets.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.EventHandlers;

public class PicklistSetChangedEventHandler :
    INotificationHandler<PicklistSetCreatedEvent>,
    INotificationHandler<PicklistSetUpdatedEvent>,
    INotificationHandler<PicklistSetDeletedEvent>
{
    private readonly ILogger<PicklistSetChangedEventHandler> _logger;
    private readonly IDataSourceService<PicklistSetDto> _picklistService;

    public PicklistSetChangedEventHandler(
        IDataSourceService<PicklistSetDto> picklistService,
        ILogger<PicklistSetChangedEventHandler> logger
    )
    {
        _picklistService = picklistService;
        _logger = logger;
    }

    public ValueTask Handle(PicklistSetCreatedEvent notification, CancellationToken cancellationToken) =>
        HandleChangeAsync(notification.GetType().Name, notification.Item.Id);

    public ValueTask Handle(PicklistSetUpdatedEvent notification, CancellationToken cancellationToken) =>
        HandleChangeAsync(notification.GetType().Name, notification.Item.Id);

    public ValueTask Handle(PicklistSetDeletedEvent notification, CancellationToken cancellationToken) =>
        HandleChangeAsync(notification.GetType().Name, notification.Item.Id);

    private async ValueTask HandleChangeAsync(string eventType, int picklistSetId)
    {
        _logger.LogInformation("Handled domain event '{EventType}' for PicklistSet ID: {PicklistSetId}",
             eventType,
             picklistSetId);
        await _picklistService.RefreshAsync();
   
    }
}
