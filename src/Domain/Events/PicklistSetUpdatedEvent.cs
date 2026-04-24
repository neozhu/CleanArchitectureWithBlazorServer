using CleanArchitecture.Blazor.Domain.Common;

namespace CleanArchitecture.Blazor.Domain.Events;

public sealed class PicklistSetUpdatedEvent : DomainEvent
{
    public PicklistSetUpdatedEvent(PicklistSet item)
    {
        Item = item;
    }

    public PicklistSet Item { get; }
}
