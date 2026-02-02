using MongoDB.Bson.Serialization.Attributes;

namespace InventorySystem.Domain.Entities;

public class Product : BaseEntity
{
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("price")]
    public decimal Price { get; set; }

    [BsonElement("stockQuantity")]
    public int StockQuantity { get; set; }

    [BsonElement("categoryId")]
    public string? CategoryId { get; set; }

    [BsonElement("sku")]
    public string? Sku { get; set; }

    [BsonElement("imageUrl")]
    public string? ImageUrl { get; set; }

    public const int LowStockThreshold = 5;

    [BsonIgnore]
    public bool IsLowStock => StockQuantity < LowStockThreshold;

    public void SetQuantity(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("La cantidad no puede ser negativa.", nameof(quantity));
        
        StockQuantity = quantity;
    }

    public void IncreaseStock(int amount)
    {
        if (amount < 0)
            throw new ArgumentException("El monto a incrementar no puede ser negativo.", nameof(amount));
        
        StockQuantity += amount;
    }

    public void DecreaseStock(int amount)
    {
        if (amount < 0)
            throw new ArgumentException("El monto a decrementar no puede ser negativo.", nameof(amount));
        
        if (StockQuantity - amount < 0)
            throw new InvalidOperationException("Stock insuficiente para realizar la operación.");
        
        StockQuantity -= amount;
    }
}
