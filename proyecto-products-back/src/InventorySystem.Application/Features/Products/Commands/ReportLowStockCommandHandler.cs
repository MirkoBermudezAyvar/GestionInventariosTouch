using MediatR;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs;
using InventorySystem.Domain.Interfaces;

namespace InventorySystem.Application.Features.Products.Commands;

public class ReportLowStockCommandHandler : IRequestHandler<ReportLowStockCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public ReportLowStockCommandHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<Result<bool>> Handle(ReportLowStockCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);

        if (product == null || !product.IsActive)
            return Result<bool>.Failure("Producto no encontrado");

        await _notificationService.NotifyLowStockAsync(product, cancellationToken);

        return Result<bool>.Success(true, "Reporte de stock bajo enviado correctamente");
    }
}
