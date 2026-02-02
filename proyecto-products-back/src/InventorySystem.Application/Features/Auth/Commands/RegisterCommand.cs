using MediatR;
using InventorySystem.Application.DTOs;
using InventorySystem.Domain.Enums;

namespace InventorySystem.Application.Features.Auth.Commands;

public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    UserRole Role = UserRole.Employee
) : IRequest<Result<AuthResponse>>;
