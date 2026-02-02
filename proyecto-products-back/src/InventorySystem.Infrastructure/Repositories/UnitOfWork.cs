using InventorySystem.Domain.Interfaces;
using InventorySystem.Infrastructure.Persistence;

namespace InventorySystem.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly MongoDbContext _context;
    private IProductRepository? _products;
    private IUserRepository? _users;
    private ICategoryRepository? _categories;
    private INotificationRepository? _notifications;

    public UnitOfWork(MongoDbContext context)
    {
        _context = context;
    }

    public IProductRepository Products => _products ??= new ProductRepository(_context);
    public IUserRepository Users => _users ??= new UserRepository(_context);
    public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
    public INotificationRepository Notifications => _notifications ??= new NotificationRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        return 1;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
