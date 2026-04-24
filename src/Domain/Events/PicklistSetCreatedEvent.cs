using CleanArchitecture.Blazor.Domain.Common;

namespace CleanArchitecture.Blazor.Domain.Events;

public sealed class PicklistSetCreatedEvent : DomainEvent
{
    public PicklistSetCreatedEvent(PicklistSet item)
    {
        Item = item;
    }

    public PicklistSet Item { get; }
}
