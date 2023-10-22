// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Events;
using CleanArchitecture.Blazor.Domain.Features.Products;

namespace CleanArchitecture.Blazor.Application.Features.Products.EventHandlers;

public class ProductDeletedEventHandler : INotificationHandler<DeletedEvent<Product>>
{
    private readonly ILogger<ProductDeletedEventHandler> _logger;

    public ProductDeletedEventHandler(
        ILogger<ProductDeletedEventHandler> logger
    )
    {
        _logger = logger;
    }

    public Task Handle(DeletedEvent<Product> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
        return Task.CompletedTask;
    }
}