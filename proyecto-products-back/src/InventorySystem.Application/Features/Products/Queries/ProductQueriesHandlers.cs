using AutoMapper;
using MediatR;
using InventorySystem.Application.DTOs;
using InventorySystem.Domain.Interfaces;

namespace InventorySystem.Application.Features.Products.Queries;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<PaginatedResult<ProductDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedResult<ProductDto>>> Handle(
        GetProductsQuery request, 
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _unitOfWork.Products.GetPaginatedAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            request.CategoryId,
            cancellationToken);

        var dtos = _mapper.Map<IEnumerable<ProductDto>>(items);

        var paginatedResult = new PaginatedResult<ProductDto>(
            dtos.ToList(),
            (int)totalCount,
            request.Page,
            request.PageSize);

        return Result<PaginatedResult<ProductDto>>.Success(paginatedResult);
    }
}

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<ProductDto>> Handle(
        GetProductByIdQuery request, 
        CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken);

        if (product == null || !product.IsActive)
            return Result<ProductDto>.Failure("Producto no encontrado");

        return Result<ProductDto>.Success(_mapper.Map<ProductDto>(product));
    }
}

public class GetLowStockProductsQueryHandler : IRequestHandler<GetLowStockProductsQuery, Result<IEnumerable<ProductDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetLowStockProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<ProductDto>>> Handle(
        GetLowStockProductsQuery request, 
        CancellationToken cancellationToken)
    {
        var products = await _unitOfWork.Products.GetLowStockProductsAsync(
            request.Threshold, 
            cancellationToken);

        return Result<IEnumerable<ProductDto>>.Success(_mapper.Map<IEnumerable<ProductDto>>(products));
    }
}
