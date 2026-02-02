using MediatR;
using InventorySystem.Application.DTOs;

namespace InventorySystem.Application.Features.Notifications.Queries;

public record GetNotificationsQuery(
    int Page = 1,
    int PageSize = 10,
    bool? OnlyUnread = null
) : IRequest<Result<PaginatedResult<NotificationDto>>>;

public record GetUnreadNotificationsCountQuery : IRequest<Result<int>>;
