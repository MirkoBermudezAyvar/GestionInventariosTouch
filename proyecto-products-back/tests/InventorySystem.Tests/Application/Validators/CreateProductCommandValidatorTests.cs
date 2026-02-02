using FluentAssertions;
using InventorySystem.Application.Features.Products.Commands;
using Xunit;

namespace InventorySystem.Tests.Application.Validators;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator;

    public CreateProductCommandValidatorTests()
    {
        _validator = new CreateProductCommandValidator();
    }

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        // Arrange
        var command = new CreateProductCommand(
            Name: "Laptop HP",
            Description: "Laptop de alta gama",
            Price: 2500.00m,
            StockQuantity: 10,
            CategoryId: "cat123",
            Sku: "LAP-001",
            ImageUrl: null
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameIsEmpty()
    {
        // Arrange
        var command = new CreateProductCommand(
            Name: "",
            Description: null,
            Price: 100m,
            StockQuantity: 10,
            CategoryId: null,
            Sku: null,
            ImageUrl: null
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_ShouldFail_WhenPriceIsZero()
    {
        // Arrange
        var command = new CreateProductCommand(
            Name: "Test Product",
            Description: null,
            Price: 0,
            StockQuantity: 10,
            CategoryId: null,
            Sku: null,
            ImageUrl: null
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Fact]
    public void Validate_ShouldFail_WhenPriceIsNegative()
    {
        // Arrange
        var command = new CreateProductCommand(
            Name: "Test Product",
            Description: null,
            Price: -50m,
            StockQuantity: 10,
            CategoryId: null,
            Sku: null,
            ImageUrl: null
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Fact]
    public void Validate_ShouldFail_WhenStockIsNegative()
    {
        // Arrange
        var command = new CreateProductCommand(
            Name: "Test Product",
            Description: null,
            Price: 100m,
            StockQuantity: -5,
            CategoryId: null,
            Sku: null,
            ImageUrl: null
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "StockQuantity");
    }

    [Fact]
    public void Validate_ShouldPass_WhenStockIsZero()
    {
        // Arrange
        var command = new CreateProductCommand(
            Name: "Test Product",
            Description: null,
            Price: 100m,
            StockQuantity: 0,
            CategoryId: null,
            Sku: null,
            ImageUrl: null
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
