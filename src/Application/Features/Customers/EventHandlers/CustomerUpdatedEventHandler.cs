// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Customers.EventHandlers;

    public class CustomerUpdatedEventHandler : INotificationHandler<CustomerUpdatedEvent>
    {
        private readonly ILogger<CustomerUpdatedEventHandler> _logger;

        public CustomerUpdatedEventHandler(
            ILogger<CustomerUpdatedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(CustomerUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
            return Task.CompletedTask;
        }
    }
