// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


namespace CleanArchitecture.Blazor.Application.Features.Products.EventHandlers;

    public class ProductCreatedEventHandler : INotificationHandler<CreatedEvent<Product>>
    {
        private readonly ILogger<ProductCreatedEventHandler> _logger;

        public ProductCreatedEventHandler(
            ILogger<ProductCreatedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(CreatedEvent<Product> notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);

            return Task.CompletedTask;
        }
    }
