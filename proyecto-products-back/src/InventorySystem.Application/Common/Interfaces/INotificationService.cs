using InventorySystem.Domain.Entities;

namespace InventorySystem.Application.Common.Interfaces;

public interface INotificationService
{
    Task NotifyLowStockAsync(Product product, CancellationToken cancellationToken = default);
    Task SendNotificationAsync(string userId, string title, string message, string type, 
        string? relatedEntityId = null, string? relatedEntityType = null, CancellationToken cancellationToken = default);
    Task SendBulkNotificationAsync(IEnumerable<string> userIds, string title, string message, 
        string type, CancellationToken cancellationToken = default);
}
