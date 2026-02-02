using InventorySystem.Domain.Entities;

namespace InventorySystem.Domain.Events;

public class LowStockEvent : IDomainEvent
{
    public Product Product { get; }
    public DateTime OccurredOn { get; }

    public LowStockEvent(Product product)
    {
        Product = product;
        OccurredOn = DateTime.UtcNow;
    }
}
