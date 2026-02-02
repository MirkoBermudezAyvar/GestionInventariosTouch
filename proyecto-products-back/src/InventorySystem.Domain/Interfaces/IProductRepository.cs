using InventorySystem.Domain.Entities;

namespace InventorySystem.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IReadOnlyList<Product>> GetLowStockProductsAsync(int threshold = 5, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetByCategoryAsync(string categoryId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Product> Items, long TotalCount)> GetPaginatedAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        string? categoryId = null,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, string? excludeId = null, CancellationToken cancellationToken = default);
    Task UpdateStockAsync(string productId, int newQuantity, CancellationToken cancellationToken = default);
}
