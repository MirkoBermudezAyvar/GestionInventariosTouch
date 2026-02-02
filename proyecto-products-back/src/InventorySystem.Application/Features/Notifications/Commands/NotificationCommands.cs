using MediatR;
using InventorySystem.Application.DTOs;

namespace InventorySystem.Application.Features.Notifications.Commands;

public record MarkNotificationAsReadCommand(string NotificationId) : IRequest<Result<bool>>;

public record MarkAllNotificationsAsReadCommand : IRequest<Result<bool>>;
