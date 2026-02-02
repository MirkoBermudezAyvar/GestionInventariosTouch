using AutoMapper;
using MediatR;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs;
using InventorySystem.Domain.Interfaces;

namespace InventorySystem.Application.Features.Notifications.Queries;

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, Result<PaginatedResult<NotificationDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetNotificationsQueryHandler(
        IUnitOfWork unitOfWork, 
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedResult<NotificationDto>>> Handle(
        GetNotificationsQuery request, 
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        
        if (string.IsNullOrEmpty(userId))
            return Result<PaginatedResult<NotificationDto>>.Failure("Usuario no autenticado");

        var (items, totalCount) = await _unitOfWork.Notifications.GetPaginatedByUserIdAsync(
            userId,
            request.Page,
            request.PageSize,
            request.OnlyUnread,
            cancellationToken);

        var dtos = _mapper.Map<IEnumerable<NotificationDto>>(items);

        var paginatedResult = new PaginatedResult<NotificationDto>(
            dtos.ToList(),
            (int)totalCount,
            request.Page,
            request.PageSize);

        return Result<PaginatedResult<NotificationDto>>.Success(paginatedResult);
    }
}

public class GetUnreadNotificationsCountQueryHandler : IRequestHandler<GetUnreadNotificationsCountQuery, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetUnreadNotificationsCountQueryHandler(
        IUnitOfWork unitOfWork, 
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(
        GetUnreadNotificationsCountQuery request, 
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        
        if (string.IsNullOrEmpty(userId))
            return Result<int>.Failure("Usuario no autenticado");

        var count = await _unitOfWork.Notifications.GetUnreadCountAsync(userId, cancellationToken);

        return Result<int>.Success(count);
    }
}
