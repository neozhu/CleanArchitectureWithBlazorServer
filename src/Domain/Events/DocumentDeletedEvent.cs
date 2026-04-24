using CleanArchitecture.Blazor.Domain.Common;

namespace CleanArchitecture.Blazor.Domain.Events;

public sealed class DocumentDeletedEvent : DomainEvent
{
    public DocumentDeletedEvent(Document item)
    {
        Item = item;
    }

    public Document Item { get; }
}
