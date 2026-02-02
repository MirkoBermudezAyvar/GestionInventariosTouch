using AutoMapper;
using MediatR;
using InventorySystem.Application.DTOs;
using InventorySystem.Domain.Interfaces;

namespace InventorySystem.Application.Features.Categories.Queries;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<IEnumerable<CategoryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<CategoryDto>>> Handle(
        GetCategoriesQuery request, 
        CancellationToken cancellationToken)
    {
        var categories = await _unitOfWork.Categories.GetAllActiveAsync(cancellationToken);
        return Result<IEnumerable<CategoryDto>>.Success(_mapper.Map<IEnumerable<CategoryDto>>(categories));
    }
}

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CategoryDto>> Handle(
        GetCategoryByIdQuery request, 
        CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.Id, cancellationToken);

        if (category == null || !category.IsActive)
            return Result<CategoryDto>.Failure("Categoría no encontrada");

        return Result<CategoryDto>.Success(_mapper.Map<CategoryDto>(category));
    }
}
