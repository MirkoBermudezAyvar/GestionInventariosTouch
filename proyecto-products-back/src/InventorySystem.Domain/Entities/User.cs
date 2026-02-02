using MongoDB.Bson.Serialization.Attributes;
using InventorySystem.Domain.Enums;

namespace InventorySystem.Domain.Entities;

public class User : BaseEntity
{
    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [BsonElement("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [BsonElement("lastName")]
    public string LastName { get; set; } = string.Empty;

    [BsonElement("role")]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public UserRole Role { get; set; } = UserRole.Employee;

    [BsonElement("refreshToken")]
    public string? RefreshToken { get; set; }

    [BsonElement("refreshTokenExpiryTime")]
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public bool IsAdmin => Role == UserRole.Administrator;
}
