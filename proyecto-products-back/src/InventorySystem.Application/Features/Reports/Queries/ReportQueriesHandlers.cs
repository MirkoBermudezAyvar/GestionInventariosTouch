using MediatR;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs;
using InventorySystem.Domain.Interfaces;

namespace InventorySystem.Application.Features.Reports.Queries;

public class GenerateLowStockReportQueryHandler : IRequestHandler<GenerateLowStockReportQuery, Result<byte[]>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPdfReportGenerator _pdfGenerator;
    private readonly ICurrentUserService _currentUserService;

    public GenerateLowStockReportQueryHandler(
        IUnitOfWork unitOfWork, 
        IPdfReportGenerator pdfGenerator,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _pdfGenerator = pdfGenerator;
        _currentUserService = currentUserService;
    }

    public async Task<Result<byte[]>> Handle(
        GenerateLowStockReportQuery request, 
        CancellationToken cancellationToken)
    {
        var products = await _unitOfWork.Products.GetLowStockProductsAsync(
            request.Threshold, 
            cancellationToken);

        var generatedBy = _currentUserService.Email ?? "Sistema";
        var pdfBytes = _pdfGenerator.GenerateLowStockReport(products, generatedBy);

        return Result<byte[]>.Success(pdfBytes);
    }
}

public class GenerateInventoryReportQueryHandler : IRequestHandler<GenerateInventoryReportQuery, Result<byte[]>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPdfReportGenerator _pdfGenerator;
    private readonly ICurrentUserService _currentUserService;

    public GenerateInventoryReportQueryHandler(
        IUnitOfWork unitOfWork, 
        IPdfReportGenerator pdfGenerator,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _pdfGenerator = pdfGenerator;
        _currentUserService = currentUserService;
    }

    public async Task<Result<byte[]>> Handle(
        GenerateInventoryReportQuery request, 
        CancellationToken cancellationToken)
    {
        var products = await _unitOfWork.Products.GetAllAsync(cancellationToken);
        var activeProducts = products.Where(p => p.IsActive).ToList();

        var generatedBy = _currentUserService.Email ?? "Sistema";
        var pdfBytes = _pdfGenerator.GenerateInventoryReport(activeProducts, generatedBy);

        return Result<byte[]>.Success(pdfBytes);
    }
}
