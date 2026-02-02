using System.Security.Claims;
using InventorySystem.Domain.Entities;

namespace InventorySystem.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    DateTime GetRefreshTokenExpiry();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
