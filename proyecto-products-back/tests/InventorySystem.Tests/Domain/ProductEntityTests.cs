using FluentAssertions;
using InventorySystem.Domain.Entities;
using Xunit;

namespace InventorySystem.Tests.Domain;

public class ProductEntityTests
{
    [Fact]
    public void Product_ShouldBeCreated_WithDefaultValues()
    {
        // Arrange & Act
        var product = new Product();

        // Assert
        product.Id.Should().NotBeNullOrEmpty();
        product.IsActive.Should().BeTrue();
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(1, true)]
    [InlineData(4, true)]
    [InlineData(5, false)]
    [InlineData(10, false)]
    [InlineData(100, false)]
    public void IsLowStock_ShouldReturnCorrectValue_BasedOnThreshold(int stockQuantity, bool expectedIsLowStock)
    {
        // Arrange
        var product = new Product { StockQuantity = stockQuantity };

        // Act
        var result = product.IsLowStock;

        // Assert
        result.Should().Be(expectedIsLowStock);
    }

    [Fact]
    public void SetQuantity_ShouldUpdateStock_WhenValidQuantity()
    {
        // Arrange
        var product = new Product { StockQuantity = 10 };

        // Act
        product.SetQuantity(25);

        // Assert
        product.StockQuantity.Should().Be(25);
    }

    [Fact]
    public void SetQuantity_ShouldThrowException_WhenNegativeQuantity()
    {
        // Arrange
        var product = new Product { StockQuantity = 10 };

        // Act
        var act = () => product.SetQuantity(-5);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("*no puede ser negativa*");
    }

    [Fact]
    public void IncreaseStock_ShouldAddToCurrentStock()
    {
        // Arrange
        var product = new Product { StockQuantity = 10 };

        // Act
        product.IncreaseStock(5);

        // Assert
        product.StockQuantity.Should().Be(15);
    }

    [Fact]
    public void IncreaseStock_ShouldThrowException_WhenNegativeAmount()
    {
        // Arrange
        var product = new Product { StockQuantity = 10 };

        // Act
        var act = () => product.IncreaseStock(-5);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void DecreaseStock_ShouldSubtractFromCurrentStock()
    {
        // Arrange
        var product = new Product { StockQuantity = 10 };

        // Act
        product.DecreaseStock(3);

        // Assert
        product.StockQuantity.Should().Be(7);
    }

    [Fact]
    public void DecreaseStock_ShouldThrowException_WhenInsufficientStock()
    {
        // Arrange
        var product = new Product { StockQuantity = 5 };

        // Act
        var act = () => product.DecreaseStock(10);

        // Assert
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Stock insuficiente*");
    }

    [Fact]
    public void DecreaseStock_ShouldThrowException_WhenNegativeAmount()
    {
        // Arrange
        var product = new Product { StockQuantity = 10 };

        // Act
        var act = () => product.DecreaseStock(-5);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
