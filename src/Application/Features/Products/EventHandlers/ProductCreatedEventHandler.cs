// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System.Diagnostics;

namespace CleanArchitecture.Blazor.Application.Features.Products.EventHandlers;

public class ProductCreatedEventHandler : INotificationHandler<CreatedEvent<Product>>
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

    public async Task Handle(CreatedEvent<Product> notification, CancellationToken cancellationToken)
    {
        _timer.Start();
        await Task.Delay(3000, cancellationToken);
        _timer.Stop();
        _logger.LogInformation("Handled domain event '{EventType}' with notification: {@Notification} in {ElapsedMilliseconds} ms", notification.GetType().Name, notification, _timer.ElapsedMilliseconds);
    }
}