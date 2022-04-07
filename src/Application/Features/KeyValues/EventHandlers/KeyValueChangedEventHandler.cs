// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.EventHandlers;

public class KeyValueChangedEventHandler : INotificationHandler<DomainEventNotification<KeyValueChangedEvent>>
{
    private readonly IPicklistService _picklistService;
    private readonly ILogger<KeyValueChangedEventHandler> _logger;

    public KeyValueChangedEventHandler(
        IPicklistService picklistService,
        ILogger<KeyValueChangedEventHandler> logger
        )
    {
        _picklistService = picklistService;
        _logger = logger;
    }
    public async Task Handle(DomainEventNotification<KeyValueChangedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        _logger.LogInformation("KeyValue Changed {DomainEvent}", domainEvent.GetType().Name);
        await _picklistService.Refresh();
    
    }
}
