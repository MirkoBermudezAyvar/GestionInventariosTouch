using MongoDB.Bson.Serialization.Attributes;

namespace InventorySystem.Domain.Entities;

public class Category : BaseEntity
{
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    public string? Description { get; set; }
}
