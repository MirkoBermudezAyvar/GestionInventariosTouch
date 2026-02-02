namespace InventorySystem.Infrastructure.Persistence;

public class MongoDbSettings
{
    public const string SectionName = "MongoDbSettings";
    
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string ProductsCollectionName { get; set; } = "products";
    public string UsersCollectionName { get; set; } = "users";
    public string CategoriesCollectionName { get; set; } = "categories";
    public string NotificationsCollectionName { get; set; } = "notifications";
}
