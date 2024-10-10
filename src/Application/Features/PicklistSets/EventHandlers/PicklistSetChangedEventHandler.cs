// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.EventHandlers;

public class PicklistSetChangedEventHandler : INotificationHandler<UpdatedEvent<PicklistSet>>
{
    private readonly ILogger<PicklistSetChangedEventHandler> _logger;
    private readonly IPicklistService _picklistService;

    public PicklistSetChangedEventHandler(
        IPicklistService picklistService,
        ILogger<PicklistSetChangedEventHandler> logger
    )
    {
        _picklistService = picklistService;
        _logger = logger;
    }

    public Task Handle(UpdatedEvent<PicklistSet> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handled domain event '{EventType}' with notification: {@Notification} ",
             notification.GetType().Name,
             notification);
        _picklistService.Refresh();
        return Task.CompletedTask;
    }
}