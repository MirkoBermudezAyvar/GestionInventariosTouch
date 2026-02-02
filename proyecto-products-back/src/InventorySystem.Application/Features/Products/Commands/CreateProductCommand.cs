using MediatR;
using InventorySystem.Application.DTOs;

namespace InventorySystem.Application.Features.Products.Commands;

public record CreateProductCommand(
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    string? CategoryId,
    string? Sku,
    string? ImageUrl
) : IRequest<Result<ProductDto>>;
