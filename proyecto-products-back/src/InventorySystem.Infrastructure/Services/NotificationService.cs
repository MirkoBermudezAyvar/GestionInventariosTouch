using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Domain.Entities;
using InventorySystem.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace InventorySystem.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IUnitOfWork unitOfWork, IEmailService emailService, ILogger<NotificationService> logger)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task NotifyLowStockAsync(Product product, CancellationToken cancellationToken = default)
    {
        var administrators = await _unitOfWork.Users.GetAdministratorsAsync(cancellationToken);

        foreach (var admin in administrators)
        {
            // Crear notificación en BD
            var notification = new Notification
            {
                UserId = admin.Id,
                Title = "Alerta de Stock Bajo",
                Message = $"El producto '{product.Name}' tiene stock bajo. Cantidad actual: {product.StockQuantity} unidades.",
                Type = "LowStock",
                RelatedEntityId = product.Id,
                RelatedEntityType = "Product"
            };

            await _unitOfWork.Notifications.AddAsync(notification, cancellationToken);
            
            // Enviar email
            await _emailService.SendLowStockAlertAsync(admin.Email, product.Name, product.StockQuantity, cancellationToken);
            
            _logger.LogInformation("Stock bajo notificado: {ProductId} -> Admin {AdminEmail}", product.Id, admin.Email);
        }
    }

    public async Task SendNotificationAsync(string userId, string title, string message, string type,
        string? relatedEntityId = null, string? relatedEntityType = null, CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            RelatedEntityId = relatedEntityId,
            RelatedEntityType = relatedEntityType
        };

        await _unitOfWork.Notifications.AddAsync(notification, cancellationToken);
        _logger.LogInformation("Notificación enviada: {Title} -> {UserId}", title, userId);
    }

    public async Task SendBulkNotificationAsync(IEnumerable<string> userIds, string title, string message,
        string type, CancellationToken cancellationToken = default)
    {
        var notifications = userIds.Select(userId => new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type
        }).ToList();

        await _unitOfWork.Notifications.AddRangeAsync(notifications, cancellationToken);
        _logger.LogInformation("Notificación masiva: {Title} -> {Count} usuarios", title, notifications.Count);
    }
}
