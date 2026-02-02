namespace InventorySystem.Application.DTOs;

public record ProductDto(
    string Id,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    string? CategoryId,
    string? Sku,
    string? ImageUrl,
    bool IsLowStock,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record ProductListDto(
    string Id,
    string Name,
    decimal Price,
    int StockQuantity,
    string? CategoryId,
    bool IsLowStock
);

public record CreateProductDto(
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    string? CategoryId,
    string? Sku,
    string? ImageUrl
);

public record UpdateProductDto(
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    string? CategoryId,
    string? Sku,
    string? ImageUrl
);
