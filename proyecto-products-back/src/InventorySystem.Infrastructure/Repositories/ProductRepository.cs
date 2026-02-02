using MongoDB.Driver;
using InventorySystem.Domain.Entities;
using InventorySystem.Domain.Interfaces;
using InventorySystem.Infrastructure.Persistence;

namespace InventorySystem.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly MongoDbContext _context;

    public ProductRepository(MongoDbContext context) : base(context.Products)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Product>> GetLowStockProductsAsync(
        int threshold = 5,
        CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(p => p.StockQuantity < threshold && p.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetByCategoryAsync(
        string categoryId,
        CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(p => p.CategoryId == categoryId && p.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Product> Items, long TotalCount)> GetPaginatedAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        string? categoryId = null,
        CancellationToken cancellationToken = default)
    {
        var filterBuilder = Builders<Product>.Filter;
        var filter = filterBuilder.Eq(p => p.IsActive, true);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchFilter = filterBuilder.Or(
                filterBuilder.Regex(p => p.Name, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                filterBuilder.Regex(p => p.Description, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            );
            filter = filterBuilder.And(filter, searchFilter);
        }

        if (!string.IsNullOrWhiteSpace(categoryId))
        {
            filter = filterBuilder.And(filter, filterBuilder.Eq(p => p.CategoryId, categoryId));
        }

        var totalCount = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var items = await _collection
            .Find(filter)
            .SortByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> ExistsByNameAsync(
        string name,
        string? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.And(
            Builders<Product>.Filter.Regex(p => p.Name, new MongoDB.Bson.BsonRegularExpression($"^{name}$", "i")),
            Builders<Product>.Filter.Eq(p => p.IsActive, true)
        );

        if (!string.IsNullOrEmpty(excludeId))
        {
            filter = Builders<Product>.Filter.And(
                filter,
                Builders<Product>.Filter.Ne(p => p.Id, excludeId)
            );
        }

        return await _collection.Find(filter).AnyAsync(cancellationToken);
    }

    public async Task UpdateStockAsync(
        string productId,
        int newQuantity,
        CancellationToken cancellationToken = default)
    {
        var update = Builders<Product>.Update
            .Set(p => p.StockQuantity, newQuantity)
            .Set(p => p.UpdatedAt, DateTime.UtcNow);

        await _collection.UpdateOneAsync(
            p => p.Id == productId,
            update,
            cancellationToken: cancellationToken);
    }
}
