using Microsoft.Extensions.Options;
using MongoDB.Driver;
using InventorySystem.Domain.Entities;

namespace InventorySystem.Infrastructure.Persistence;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    private readonly MongoDbSettings _settings;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        _settings = settings.Value;
        var client = new MongoClient(_settings.ConnectionString);
        _database = client.GetDatabase(_settings.DatabaseName);
        CreateIndexes();
    }

    public IMongoCollection<Product> Products => _database.GetCollection<Product>(_settings.ProductsCollectionName);
    public IMongoCollection<User> Users => _database.GetCollection<User>(_settings.UsersCollectionName);
    public IMongoCollection<Category> Categories => _database.GetCollection<Category>(_settings.CategoriesCollectionName);
    public IMongoCollection<Notification> Notifications => _database.GetCollection<Notification>(_settings.NotificationsCollectionName);

    private void CreateIndexes()
    {
        Products.Indexes.CreateOne(new CreateIndexModel<Product>(Builders<Product>.IndexKeys.Ascending(p => p.Name)));
        Products.Indexes.CreateOne(new CreateIndexModel<Product>(Builders<Product>.IndexKeys.Ascending(p => p.CategoryId)));
        Products.Indexes.CreateOne(new CreateIndexModel<Product>(Builders<Product>.IndexKeys.Ascending(p => p.StockQuantity)));

        Users.Indexes.CreateOne(new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Ascending(u => u.Email),
            new CreateIndexOptions { Unique = true }));

        Categories.Indexes.CreateOne(new CreateIndexModel<Category>(
            Builders<Category>.IndexKeys.Ascending(c => c.Name),
            new CreateIndexOptions { Unique = true }));

        Notifications.Indexes.CreateOne(new CreateIndexModel<Notification>(Builders<Notification>.IndexKeys.Ascending(n => n.UserId)));
        Notifications.Indexes.CreateOne(new CreateIndexModel<Notification>(Builders<Notification>.IndexKeys.Ascending(n => n.IsRead)));
    }
}
