using InventorySystem.Domain.Entities;

namespace InventorySystem.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task UpdateRefreshTokenAsync(string userId, string refreshToken, DateTime refreshTokenExpiry, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetAdministratorsAsync(CancellationToken cancellationToken = default);
}
