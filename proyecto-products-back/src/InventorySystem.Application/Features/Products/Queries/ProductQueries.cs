using MediatR;
using InventorySystem.Application.DTOs;

namespace InventorySystem.Application.Features.Products.Queries;

public record GetProductsQuery(
    int Page = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? CategoryId = null
) : IRequest<Result<PaginatedResult<ProductDto>>>;

public record GetProductByIdQuery(string Id) : IRequest<Result<ProductDto>>;

public record GetLowStockProductsQuery(int Threshold = 5) : IRequest<Result<IEnumerable<ProductDto>>>;
