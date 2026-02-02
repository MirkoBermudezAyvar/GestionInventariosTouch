using MediatR;
using InventorySystem.Application.DTOs;

namespace InventorySystem.Application.Features.Reports.Queries;

public record GenerateLowStockReportQuery(int Threshold = 5) : IRequest<Result<byte[]>>;

public record GenerateInventoryReportQuery : IRequest<Result<byte[]>>;
