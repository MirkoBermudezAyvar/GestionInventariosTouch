using AutoMapper;
using MediatR;
using InventorySystem.Application.DTOs;
using InventorySystem.Domain.Entities;
using InventorySystem.Domain.Interfaces;

namespace InventorySystem.Application.Features.Categories.Commands;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CategoryDto>> Handle(
        CreateCategoryCommand request, 
        CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Categories.ExistsByNameAsync(request.Name, null, cancellationToken))
            return Result<CategoryDto>.Failure("Ya existe una categoría con ese nombre");

        var category = new Category
        {
            Name = request.Name,
            Description = request.Description
        };

        await _unitOfWork.Categories.AddAsync(category, cancellationToken);

        return Result<CategoryDto>.Success(_mapper.Map<CategoryDto>(category), "Categoría creada correctamente");
    }
}

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<CategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CategoryDto>> Handle(
        UpdateCategoryCommand request, 
        CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.Id, cancellationToken);

        if (category == null || !category.IsActive)
            return Result<CategoryDto>.Failure("Categoría no encontrada");

        if (await _unitOfWork.Categories.ExistsByNameAsync(request.Name, request.Id, cancellationToken))
            return Result<CategoryDto>.Failure("Ya existe otra categoría con ese nombre");

        category.Name = request.Name;
        category.Description = request.Description;

        await _unitOfWork.Categories.UpdateAsync(category, cancellationToken);

        return Result<CategoryDto>.Success(_mapper.Map<CategoryDto>(category), "Categoría actualizada correctamente");
    }
}

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        DeleteCategoryCommand request, 
        CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.Id, cancellationToken);

        if (category == null || !category.IsActive)
            return Result<bool>.Failure("Categoría no encontrada");

        // Verificar si hay productos en esta categoría
        var products = await _unitOfWork.Products.GetByCategoryAsync(request.Id, cancellationToken);
        if (products.Any())
            return Result<bool>.Failure("No se puede eliminar la categoría porque tiene productos asociados");

        category.IsActive = false;
        await _unitOfWork.Categories.UpdateAsync(category, cancellationToken);

        return Result<bool>.Success(true, "Categoría eliminada correctamente");
    }
}
