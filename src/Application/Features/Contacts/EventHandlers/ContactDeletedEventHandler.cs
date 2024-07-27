// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Contacts.EventHandlers;

    public class ContactDeletedEventHandler : INotificationHandler<ContactDeletedEvent>
    {
        private readonly ILogger<ContactDeletedEventHandler> _logger;

        public ContactDeletedEventHandler(
            ILogger<ContactDeletedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(ContactDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
            return Task.CompletedTask;
        }
    }
