// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Customers.EventHandlers;

    public class CustomerDeletedEventHandler : INotificationHandler<DeletedEvent<Customer>>
    {
        private readonly ILogger<CustomerDeletedEventHandler> _logger;

        public CustomerDeletedEventHandler(
            ILogger<CustomerDeletedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(DeletedEvent<Customer> notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
            return Task.CompletedTask;
        }
    }
