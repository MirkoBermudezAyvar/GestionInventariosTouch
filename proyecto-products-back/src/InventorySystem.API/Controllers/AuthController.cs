using Microsoft.AspNetCore.Mvc;
using InventorySystem.Application.Features.Auth.Commands;
using InventorySystem.Application.DTOs;

namespace InventorySystem.API.Controllers;

public class AuthController : BaseApiController
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(Result<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await Mediator.Send(command);
        return !result.IsSuccess ? BadRequest(result) : Ok(result);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(Result<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await Mediator.Send(command);
        return !result.IsSuccess ? Unauthorized(result) : Ok(result);
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(Result<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var result = await Mediator.Send(command);
        return !result.IsSuccess ? Unauthorized(result) : Ok(result);
    }
}
