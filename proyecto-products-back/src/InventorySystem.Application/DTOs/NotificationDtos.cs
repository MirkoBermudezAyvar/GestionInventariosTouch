namespace InventorySystem.Application.DTOs;

public record NotificationDto(
    string Id,
    string UserId,
    string Title,
    string Message,
    string Type,
    bool IsRead,
    string? RelatedEntityId,
    string? RelatedEntityType,
    DateTime CreatedAt,
    DateTime? ReadAt
);
