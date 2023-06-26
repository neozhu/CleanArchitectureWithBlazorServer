// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;

namespace CleanArchitecture.Blazor.Application.Features.Tenants.EventHandlers;

public class TenantChangedEventHandler : INotificationHandler<UpdatedEvent<Tenant>>
{
    private readonly ILogger<TenantChangedEventHandler> _logger;
    private readonly ITenantService _tenantsService;

    public TenantChangedEventHandler(
        ITenantService tenantsService,
        ILogger<TenantChangedEventHandler> logger
    )
    {
        _tenantsService = tenantsService;
        _logger = logger;
    }

    public async Task Handle(UpdatedEvent<Tenant> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Tenant Changed {DomainEvent}, {@Entity}", nameof(notification), notification.Entity);
        await _tenantsService.Refresh();
    }
}