using AutoMapper;
using MediatR;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs;
using InventorySystem.Domain.Entities;
using InventorySystem.Domain.Interfaces;

namespace InventorySystem.Application.Features.Products.Commands;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (request.StockQuantity < 0)
            return Result<ProductDto>.Failure("La cantidad en inventario no puede ser negativa.");

        if (await _unitOfWork.Products.ExistsByNameAsync(request.Name, null, cancellationToken))
            return Result<ProductDto>.Failure($"Ya existe un producto con el nombre '{request.Name}'.");

        if (!string.IsNullOrEmpty(request.CategoryId))
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId, cancellationToken);
            if (category == null || !category.IsActive)
                return Result<ProductDto>.Failure("La categoría especificada no existe.");
        }

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            CategoryId = request.CategoryId,
            Sku = request.Sku,
            ImageUrl = request.ImageUrl
        };

        await _unitOfWork.Products.AddAsync(product, cancellationToken);

        if (product.IsLowStock)
            await _notificationService.NotifyLowStockAsync(product, cancellationToken);

        return Result<ProductDto>.Success(_mapper.Map<ProductDto>(product), "Producto creado correctamente");
    }
}
