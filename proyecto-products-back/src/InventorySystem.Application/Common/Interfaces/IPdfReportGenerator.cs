using InventorySystem.Domain.Entities;

namespace InventorySystem.Application.Common.Interfaces;

public interface IPdfReportGenerator
{
    byte[] GenerateLowStockReport(IEnumerable<Product> products, string generatedBy);
    byte[] GenerateInventoryReport(IEnumerable<Product> products, string generatedBy);
}
