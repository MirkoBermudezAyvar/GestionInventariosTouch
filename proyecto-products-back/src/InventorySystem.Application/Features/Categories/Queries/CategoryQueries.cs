using MediatR;
using InventorySystem.Application.DTOs;

namespace InventorySystem.Application.Features.Categories.Queries;

public record GetCategoriesQuery : IRequest<Result<IEnumerable<CategoryDto>>>;

public record GetCategoryByIdQuery(string Id) : IRequest<Result<CategoryDto>>;
