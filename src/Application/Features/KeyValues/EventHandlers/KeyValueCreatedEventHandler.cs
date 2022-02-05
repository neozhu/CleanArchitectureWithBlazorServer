// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Razor.Application.Features.KeyValues.EventHandlers;

public class KeyValueCreatedEventHandler : INotificationHandler<DomainEventNotification<KeyValueCreatedEvent>>
{
    private readonly ILogger<KeyValueCreatedEventHandler> _logger;

    public KeyValueCreatedEventHandler(
        ILogger<KeyValueCreatedEventHandler> logger
        )
    {
        _logger = logger;
    }
    public Task Handle(DomainEventNotification<KeyValueCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", domainEvent.GetType().Name);

        return Task.CompletedTask;
    }
}
