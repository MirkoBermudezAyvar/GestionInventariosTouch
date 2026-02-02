using InventorySystem.Domain.Entities;

namespace InventorySystem.Domain.Interfaces;

public interface INotificationRepository : IRepository<Notification>
{
    Task<IReadOnlyList<Notification>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Notification>> GetUnreadByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(string userId, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(string notificationId, CancellationToken cancellationToken = default);
    Task MarkAllAsReadAsync(string userId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Notification> Items, long TotalCount)> GetPaginatedByUserIdAsync(
        string userId,
        int page,
        int pageSize,
        bool? onlyUnread = null,
        CancellationToken cancellationToken = default);
}
