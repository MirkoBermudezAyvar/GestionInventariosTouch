using FluentAssertions;
using FluentValidation.TestHelper;
using InventorySystem.Application.Features.Products.Commands;
using Xunit;

namespace InventorySystem.Tests.Products;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = new CreateProductCommand("Producto Válido", "Descripción válida", 100.00m, 10, "cat-123", "SKU-001", null);
        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyName_FailsValidation()
    {
        var command = new CreateProductCommand("", "Descripción", 100.00m, 10, null, "SKU-001", null);
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_NameExceedsMaxLength_FailsValidation()
    {
        var command = new CreateProductCommand(new string('a', 201), "Descripción", 100.00m, 10, null, "SKU-001", null);
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ZeroPrice_FailsValidation()
    {
        var command = new CreateProductCommand("Producto", "Descripción", 0m, 10, null, "SKU-001", null);
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Validate_NegativePrice_FailsValidation()
    {
        var command = new CreateProductCommand("Producto", "Descripción", -50m, 10, null, "SKU-001", null);
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Validate_NegativeQuantity_FailsValidation()
    {
        var command = new CreateProductCommand("Producto", "Descripción", 100m, -5, null, "SKU-001", null);
        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.StockQuantity);
    }

    [Fact]
    public void Validate_ZeroQuantity_PassesValidation()
    {
        var command = new CreateProductCommand("Producto", "Descripción", 100m, 0, null, "SKU-001", null);
        _validator.TestValidate(command).ShouldNotHaveValidationErrorFor(x => x.StockQuantity);
    }
}
