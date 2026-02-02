using FluentAssertions;
using InventorySystem.Domain.Entities;
using InventorySystem.Domain.Enums;
using Xunit;

namespace InventorySystem.Tests.Domain;

public class UserEntityTests
{
    [Fact]
    public void User_ShouldBeCreated_WithDefaultValues()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        user.Id.Should().NotBeNullOrEmpty();
        user.IsActive.Should().BeTrue();
        user.Role.Should().Be(UserRole.Employee);
    }

    [Fact]
    public void FullName_ShouldCombineFirstAndLastName()
    {
        // Arrange
        var user = new User
        {
            FirstName = "Juan",
            LastName = "Pérez"
        };

        // Act
        var fullName = user.FullName;

        // Assert
        fullName.Should().Be("Juan Pérez");
    }

    [Fact]
    public void IsRefreshTokenValid_ShouldReturnTrue_WhenTokenMatchesAndNotExpired()
    {
        // Arrange
        var user = new User
        {
            RefreshToken = "valid-token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var isValid = user.IsRefreshTokenValid("valid-token");

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void IsRefreshTokenValid_ShouldReturnFalse_WhenTokenDoesNotMatch()
    {
        // Arrange
        var user = new User
        {
            RefreshToken = "valid-token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var isValid = user.IsRefreshTokenValid("invalid-token");

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void IsRefreshTokenValid_ShouldReturnFalse_WhenTokenExpired()
    {
        // Arrange
        var user = new User
        {
            RefreshToken = "valid-token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1) // Expirado
        };

        // Act
        var isValid = user.IsRefreshTokenValid("valid-token");

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void IsRefreshTokenValid_ShouldReturnFalse_WhenNoExpiryTime()
    {
        // Arrange
        var user = new User
        {
            RefreshToken = "valid-token",
            RefreshTokenExpiryTime = null
        };

        // Act
        var isValid = user.IsRefreshTokenValid("valid-token");

        // Assert
        isValid.Should().BeFalse();
    }
}
