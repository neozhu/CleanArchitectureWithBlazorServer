using CleanArchitecture.Blazor.Domain.Common;

namespace CleanArchitecture.Blazor.Domain.Events;

public sealed class DocumentCreatedEvent : DomainEvent
{
    public DocumentCreatedEvent(Document item)
    {
        Item = item;
    }

    public Document Item { get; }
}
