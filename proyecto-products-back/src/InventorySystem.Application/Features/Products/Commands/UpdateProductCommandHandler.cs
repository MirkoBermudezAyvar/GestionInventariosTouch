using AutoMapper;
using MediatR;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs;
using InventorySystem.Domain.Interfaces;

namespace InventorySystem.Application.Features.Products.Commands;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<Result<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        if (request.StockQuantity < 0)
            return Result<ProductDto>.Failure("La cantidad en inventario no puede ser negativa.");

        var product = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken);

        if (product == null || !product.IsActive)
            return Result<ProductDto>.Failure("Producto no encontrado.");

        if (await _unitOfWork.Products.ExistsByNameAsync(request.Name, request.Id, cancellationToken))
            return Result<ProductDto>.Failure($"Ya existe otro producto con el nombre '{request.Name}'.");

        if (!string.IsNullOrEmpty(request.CategoryId))
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId, cancellationToken);
            if (category == null || !category.IsActive)
                return Result<ProductDto>.Failure("La categoría especificada no existe.");
        }

        var wasLowStock = product.IsLowStock;

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;
        product.CategoryId = request.CategoryId;
        product.Sku = request.Sku;
        product.ImageUrl = request.ImageUrl;

        await _unitOfWork.Products.UpdateAsync(product, cancellationToken);

        if (!wasLowStock && product.IsLowStock)
            await _notificationService.NotifyLowStockAsync(product, cancellationToken);

        return Result<ProductDto>.Success(_mapper.Map<ProductDto>(product), "Producto actualizado correctamente");
    }
}
