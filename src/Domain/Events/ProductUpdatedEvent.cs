using CleanArchitecture.Blazor.Domain.Common;

namespace CleanArchitecture.Blazor.Domain.Events;

public sealed class ProductUpdatedEvent : DomainEvent
{
    public ProductUpdatedEvent(Product item)
    {
        Item = item;
    }

    public Product Item { get; }
}
