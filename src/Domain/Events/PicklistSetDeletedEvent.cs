using CleanArchitecture.Blazor.Domain.Common;

namespace CleanArchitecture.Blazor.Domain.Events;

public sealed class PicklistSetDeletedEvent : DomainEvent
{
    public PicklistSetDeletedEvent(PicklistSet item)
    {
        Item = item;
    }

    public PicklistSet Item { get; }
}
