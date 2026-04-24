// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System.Diagnostics;

namespace CleanArchitecture.Blazor.Application.Features.Products.EventHandlers;

public class ProductCreatedEventHandler : INotificationHandler<ProductCreatedEvent>
{
    private readonly ILogger<ProductCreatedEventHandler> _logger;
    private readonly Stopwatch _timer;

    public ProductCreatedEventHandler(
        ILogger<ProductCreatedEventHandler> logger
    )
    {
        _logger = logger;
        _timer = new Stopwatch();
    }

    public async ValueTask Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
    {
        _timer.Start();
        await Task.Delay(3000, cancellationToken);
        _timer.Stop();
        _logger.LogInformation("Handled domain event '{EventType}' for Product ID: {ProductId} in {ElapsedMilliseconds} ms", 
            notification.GetType().Name, notification.Item.Id, _timer.ElapsedMilliseconds);
    }
}
