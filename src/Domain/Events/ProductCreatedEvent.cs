using CleanArchitecture.Blazor.Domain.Common;

namespace CleanArchitecture.Blazor.Domain.Events;

public sealed class ProductCreatedEvent : DomainEvent
{
    public ProductCreatedEvent(Product item)
    {
        Item = item;
    }

    public Product Item { get; }
}
