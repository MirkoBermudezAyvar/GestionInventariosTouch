using MediatR;
using InventorySystem.Application.DTOs;

namespace InventorySystem.Application.Features.Products.Commands;

public record DeleteProductCommand(string Id) : IRequest<Result<bool>>;
