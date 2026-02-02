using MongoDB.Driver;
using InventorySystem.Domain.Entities;
using InventorySystem.Domain.Interfaces;
using InventorySystem.Infrastructure.Persistence;

namespace InventorySystem.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(MongoDbContext context) : base(context.Users)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(u => u.Email.ToLower() == email.ToLower() && u.IsActive)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(u => u.Email.ToLower() == email.ToLower())
            .AnyAsync(cancellationToken);
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(u => u.RefreshToken == refreshToken && u.IsActive)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateRefreshTokenAsync(
        string userId,
        string refreshToken,
        DateTime refreshTokenExpiry,
        CancellationToken cancellationToken = default)
    {
        var update = Builders<User>.Update
            .Set(u => u.RefreshToken, refreshToken)
            .Set(u => u.RefreshTokenExpiryTime, refreshTokenExpiry)
            .Set(u => u.UpdatedAt, DateTime.UtcNow);

        await _collection.UpdateOneAsync(
            u => u.Id == userId,
            update,
            cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAdministratorsAsync(CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(u => u.Role == Domain.Enums.UserRole.Administrator && u.IsActive)
            .ToListAsync(cancellationToken);
    }
}
