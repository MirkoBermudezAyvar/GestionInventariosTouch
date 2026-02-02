using FluentAssertions;
using InventorySystem.Application.Features.Auth.Commands;
using Xunit;

namespace InventorySystem.Tests.Application.Validators;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator;

    public RegisterCommandValidatorTests()
    {
        _validator = new RegisterCommandValidator();
    }

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        // Arrange
        var command = new RegisterCommand(
            Email: "test@example.com",
            Password: "Password123",
            FirstName: "Juan",
            LastName: "Pérez"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFail_WhenEmailIsEmpty()
    {
        // Arrange
        var command = new RegisterCommand(
            Email: "",
            Password: "Password123",
            FirstName: "Juan",
            LastName: "Pérez"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validate_ShouldFail_WhenEmailIsInvalid()
    {
        // Arrange
        var command = new RegisterCommand(
            Email: "invalid-email",
            Password: "Password123",
            FirstName: "Juan",
            LastName: "Pérez"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validate_ShouldFail_WhenPasswordIsTooShort()
    {
        // Arrange
        var command = new RegisterCommand(
            Email: "test@example.com",
            Password: "Ab1",
            FirstName: "Juan",
            LastName: "Pérez"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void Validate_ShouldFail_WhenPasswordHasNoUppercase()
    {
        // Arrange
        var command = new RegisterCommand(
            Email: "test@example.com",
            Password: "password123",
            FirstName: "Juan",
            LastName: "Pérez"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Password" && e.ErrorMessage.Contains("mayúscula"));
    }

    [Fact]
    public void Validate_ShouldFail_WhenPasswordHasNoLowercase()
    {
        // Arrange
        var command = new RegisterCommand(
            Email: "test@example.com",
            Password: "PASSWORD123",
            FirstName: "Juan",
            LastName: "Pérez"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Password" && e.ErrorMessage.Contains("minúscula"));
    }

    [Fact]
    public void Validate_ShouldFail_WhenPasswordHasNoNumber()
    {
        // Arrange
        var command = new RegisterCommand(
            Email: "test@example.com",
            Password: "PasswordABC",
            FirstName: "Juan",
            LastName: "Pérez"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Password" && e.ErrorMessage.Contains("número"));
    }

    [Fact]
    public void Validate_ShouldFail_WhenFirstNameIsEmpty()
    {
        // Arrange
        var command = new RegisterCommand(
            Email: "test@example.com",
            Password: "Password123",
            FirstName: "",
            LastName: "Pérez"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FirstName");
    }

    [Fact]
    public void Validate_ShouldFail_WhenLastNameIsEmpty()
    {
        // Arrange
        var command = new RegisterCommand(
            Email: "test@example.com",
            Password: "Password123",
            FirstName: "Juan",
            LastName: ""
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "LastName");
    }
}
