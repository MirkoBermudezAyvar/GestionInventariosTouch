using InventorySystem.Domain.Enums;

namespace InventorySystem.Application.DTOs;

public record UserDto(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    UserRole Role,
    DateTime CreatedAt
);

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    UserDto User,
    DateTime ExpiresAt
);
