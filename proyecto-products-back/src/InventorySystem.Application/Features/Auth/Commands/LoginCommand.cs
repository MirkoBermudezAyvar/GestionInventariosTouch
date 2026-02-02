using MediatR;
using InventorySystem.Application.DTOs;

namespace InventorySystem.Application.Features.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponse>>;
