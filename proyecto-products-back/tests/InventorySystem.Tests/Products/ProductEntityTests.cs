using FluentAssertions;
using InventorySystem.Domain.Entities;
using Xunit;

namespace InventorySystem.Tests.Products;

public class ProductEntityTests
{
    [Fact]
    public void IsLowStock_WhenQuantityBelowThreshold_ReturnsTrue()
    {
        var product = new Product { Name = "Test Product", StockQuantity = 3 };
        product.IsLowStock.Should().BeTrue();
    }

    [Fact]
    public void IsLowStock_WhenQuantityAboveThreshold_ReturnsFalse()
    {
        var product = new Product { Name = "Test Product", StockQuantity = 10 };
        product.IsLowStock.Should().BeFalse();
    }

    [Fact]
    public void SetQuantity_WithValidValue_SetsQuantity()
    {
        var product = new Product { Name = "Test" };
        product.SetQuantity(50);
        product.StockQuantity.Should().Be(50);
    }

    [Fact]
    public void SetQuantity_WithNegativeValue_ThrowsException()
    {
        var product = new Product { Name = "Test" };
        var action = () => product.SetQuantity(-5);
        action.Should().Throw<ArgumentException>().WithMessage("*negativa*");
    }

    [Fact]
    public void IncreaseStock_WithValidAmount_IncreasesQuantity()
    {
        var product = new Product { Name = "Test", StockQuantity = 10 };
        product.IncreaseStock(5);
        product.StockQuantity.Should().Be(15);
    }

    [Fact]
    public void DecreaseStock_WithValidAmount_DecreasesQuantity()
    {
        var product = new Product { Name = "Test", StockQuantity = 10 };
        product.DecreaseStock(3);
        product.StockQuantity.Should().Be(7);
    }

    [Fact]
    public void DecreaseStock_WithInsufficientStock_ThrowsException()
    {
        var product = new Product { Name = "Test", StockQuantity = 5 };
        var action = () => product.DecreaseStock(10);
        action.Should().Throw<InvalidOperationException>().WithMessage("*insuficiente*");
    }
}
