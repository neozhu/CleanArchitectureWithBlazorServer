// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.EventHandlers;

public class TenantChangedEventHandler : INotificationHandler<DomainEventNotification<UpdatedEvent<Tenant>>>
{
    private readonly ITenantsService  _tenantsService;
    private readonly ILogger<KeyValueChangedEventHandler> _logger;

    public TenantChangedEventHandler(
        ITenantsService tenantsService,
        ILogger<KeyValueChangedEventHandler> logger
        )
    {
        _tenantsService = tenantsService;
        _logger = logger;
    }
    public async Task Handle(DomainEventNotification<UpdatedEvent<Tenant>> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        _logger.LogInformation("Tenant Changed {DomainEvent},{Entity}", nameof(domainEvent),domainEvent.Entity);
        await _tenantsService.Refresh();
    
    }
}
