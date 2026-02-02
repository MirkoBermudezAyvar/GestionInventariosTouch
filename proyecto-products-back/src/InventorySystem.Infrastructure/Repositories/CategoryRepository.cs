using MongoDB.Driver;
using InventorySystem.Domain.Entities;
using InventorySystem.Domain.Interfaces;
using InventorySystem.Infrastructure.Persistence;

namespace InventorySystem.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(MongoDbContext context) : base(context.Categories)
    {
    }

    public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(c => c.Name.ToLower() == name.ToLower() && c.IsActive)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        string name,
        string? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<Category>.Filter.And(
            Builders<Category>.Filter.Regex(c => c.Name, new MongoDB.Bson.BsonRegularExpression($"^{name}$", "i")),
            Builders<Category>.Filter.Eq(c => c.IsActive, true)
        );

        if (!string.IsNullOrEmpty(excludeId))
        {
            filter = Builders<Category>.Filter.And(
                filter,
                Builders<Category>.Filter.Ne(c => c.Id, excludeId)
            );
        }

        return await _collection.Find(filter).AnyAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(c => c.IsActive)
            .SortBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }
}
