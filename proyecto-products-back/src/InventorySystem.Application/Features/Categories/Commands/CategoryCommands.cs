using MediatR;
using InventorySystem.Application.DTOs;

namespace InventorySystem.Application.Features.Categories.Commands;

public record CreateCategoryCommand(string Name, string? Description) : IRequest<Result<CategoryDto>>;

public record UpdateCategoryCommand(string Id, string Name, string? Description) : IRequest<Result<CategoryDto>>;

public record DeleteCategoryCommand(string Id) : IRequest<Result<bool>>;
