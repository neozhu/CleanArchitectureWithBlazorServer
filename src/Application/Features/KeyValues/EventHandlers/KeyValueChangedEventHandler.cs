// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.EventHandlers;

public class KeyValueChangedEventHandler : INotificationHandler<UpdatedEvent<KeyValue>>
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
    public async Task Handle(UpdatedEvent<KeyValue> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("KeyValue Changed {DomainEvent},{@Entity}", nameof(notification), notification.Entity);
        await _picklistService.Refresh();
    
    }
}
