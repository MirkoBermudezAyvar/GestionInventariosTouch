using MongoDB.Driver;
using InventorySystem.Domain.Entities;
using InventorySystem.Domain.Interfaces;
using InventorySystem.Infrastructure.Persistence;

namespace InventorySystem.Infrastructure.Repositories;

public class NotificationRepository : Repository<Notification>, INotificationRepository
{
    public NotificationRepository(MongoDbContext context) : base(context.Notifications)
    {
    }

    public async Task<IReadOnlyList<Notification>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(n => n.UserId == userId)
            .SortByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> GetUnreadByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(n => n.UserId == userId && !n.IsRead)
            .SortByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(string userId, CancellationToken cancellationToken = default)
    {
        return (int)await _collection
            .CountDocumentsAsync(n => n.UserId == userId && !n.IsRead, cancellationToken: cancellationToken);
    }

    public async Task MarkAsReadAsync(string notificationId, CancellationToken cancellationToken = default)
    {
        var update = Builders<Notification>.Update
            .Set(n => n.IsRead, true)
            .Set(n => n.ReadAt, DateTime.UtcNow)
            .Set(n => n.UpdatedAt, DateTime.UtcNow);

        await _collection.UpdateOneAsync(
            n => n.Id == notificationId,
            update,
            cancellationToken: cancellationToken);
    }

    public async Task MarkAllAsReadAsync(string userId, CancellationToken cancellationToken = default)
    {
        var update = Builders<Notification>.Update
            .Set(n => n.IsRead, true)
            .Set(n => n.ReadAt, DateTime.UtcNow)
            .Set(n => n.UpdatedAt, DateTime.UtcNow);

        await _collection.UpdateManyAsync(
            n => n.UserId == userId && !n.IsRead,
            update,
            cancellationToken: cancellationToken);
    }

    public async Task<(IReadOnlyList<Notification> Items, long TotalCount)> GetPaginatedByUserIdAsync(
        string userId,
        int page,
        int pageSize,
        bool? onlyUnread = null,
        CancellationToken cancellationToken = default)
    {
        var filterBuilder = Builders<Notification>.Filter;
        var filter = filterBuilder.Eq(n => n.UserId, userId);

        if (onlyUnread == true)
        {
            filter = filterBuilder.And(filter, filterBuilder.Eq(n => n.IsRead, false));
        }

        var totalCount = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var items = await _collection
            .Find(filter)
            .SortByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
