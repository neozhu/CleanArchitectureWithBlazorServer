// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


namespace CleanArchitecture.Blazor.Application.Features.Products.EventHandlers;

public class ProductUpdatedEventHandler : INotificationHandler<ProductUpdatedEvent>
{
    private readonly ILogger<ProductUpdatedEventHandler> _logger;

    public ProductUpdatedEventHandler(
        ILogger<ProductUpdatedEventHandler> logger
    )
    {
        _logger = logger;
    }

    public ValueTask Handle(ProductUpdatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handled domain event '{EventType}' for Product ID: {ProductId}", 
            notification.GetType().Name, notification.Item.Id);

        return ValueTask.CompletedTask;
    }
}
