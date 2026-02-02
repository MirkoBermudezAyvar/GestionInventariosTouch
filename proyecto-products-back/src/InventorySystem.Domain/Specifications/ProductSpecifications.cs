using System.Linq.Expressions;
using InventorySystem.Domain.Entities;

namespace InventorySystem.Domain.Specifications;

public class ProductLowStockSpecification : Specification<Product>
{
    public override Expression<Func<Product, bool>> Criteria =>
        product => product.StockQuantity < Product.LowStockThreshold && product.IsActive;
}

public class ProductByCategorySpecification : Specification<Product>
{
    private readonly string _categoryId;

    public ProductByCategorySpecification(string categoryId) => _categoryId = categoryId;

    public override Expression<Func<Product, bool>> Criteria =>
        product => product.CategoryId == _categoryId && product.IsActive;
}

public class ProductByNameSpecification : Specification<Product>
{
    private readonly string _name;

    public ProductByNameSpecification(string name) => _name = name;

    public override Expression<Func<Product, bool>> Criteria =>
        product => product.Name.ToLower().Contains(_name.ToLower()) && product.IsActive;
}

public class ActiveProductSpecification : Specification<Product>
{
    public override Expression<Func<Product, bool>> Criteria => product => product.IsActive;
}
