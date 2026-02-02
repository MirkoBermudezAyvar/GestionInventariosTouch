using MediatR;
using InventorySystem.Application.DTOs;

namespace InventorySystem.Application.Features.Auth.Commands;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<Result<AuthResponse>>;
