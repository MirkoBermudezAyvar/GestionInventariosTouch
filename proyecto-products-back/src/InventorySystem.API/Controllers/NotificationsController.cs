using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventorySystem.Application.DTOs;
using InventorySystem.Application.Features.Notifications.Commands;
using InventorySystem.Application.Features.Notifications.Queries;

namespace InventorySystem.API.Controllers;

[Authorize]
public class NotificationsController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(Result<PaginatedResult<NotificationDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNotifications([FromQuery] GetNotificationsQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnreadCount()
    {
        return Ok(await Mediator.Send(new GetUnreadNotificationsCountQuery()));
    }

    [HttpPut("{id}/read")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(string id)
    {
        var result = await Mediator.Send(new MarkNotificationAsReadCommand(id));
        return !result.IsSuccess ? NotFound(result) : Ok(result);
    }

    [HttpPut("read-all")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        return Ok(await Mediator.Send(new MarkAllNotificationsAsReadCommand()));
    }
}
