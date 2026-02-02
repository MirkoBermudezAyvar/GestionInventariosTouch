using MediatR;
using InventorySystem.Application.DTOs;

namespace InventorySystem.Application.Features.Products.Commands;

public record ReportLowStockCommand(string ProductId) : IRequest<Result<bool>>;
