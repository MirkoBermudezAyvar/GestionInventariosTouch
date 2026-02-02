using MediatR;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs;
using InventorySystem.Domain.Interfaces;

namespace InventorySystem.Application.Features.Notifications.Commands;

public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public MarkNotificationAsReadCommandHandler(
        IUnitOfWork unitOfWork, 
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(
        MarkNotificationAsReadCommand request, 
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        
        if (string.IsNullOrEmpty(userId))
            return Result<bool>.Failure("Usuario no autenticado");

        var notification = await _unitOfWork.Notifications.GetByIdAsync(request.NotificationId, cancellationToken);

        if (notification == null)
            return Result<bool>.Failure("Notificación no encontrada");

        if (notification.UserId != userId)
            return Result<bool>.Failure("No tiene permiso para acceder a esta notificación");

        await _unitOfWork.Notifications.MarkAsReadAsync(request.NotificationId, cancellationToken);

        return Result<bool>.Success(true, "Notificación marcada como leída");
    }
}

public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public MarkAllNotificationsAsReadCommandHandler(
        IUnitOfWork unitOfWork, 
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(
        MarkAllNotificationsAsReadCommand request, 
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        
        if (string.IsNullOrEmpty(userId))
            return Result<bool>.Failure("Usuario no autenticado");

        await _unitOfWork.Notifications.MarkAllAsReadAsync(userId, cancellationToken);

        return Result<bool>.Success(true, "Todas las notificaciones marcadas como leídas");
    }
}
