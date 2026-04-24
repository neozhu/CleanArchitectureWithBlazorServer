using CleanArchitecture.Blazor.Domain.Common;

namespace CleanArchitecture.Blazor.Domain.Events;

public sealed class ProductDeletedEvent : DomainEvent
{
    public ProductDeletedEvent(Product item)
    {
        Item = item;
    }

    public Product Item { get; }
}
