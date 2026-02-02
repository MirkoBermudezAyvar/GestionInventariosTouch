using FluentAssertions;
using InventorySystem.Infrastructure.Security;
using Xunit;

namespace InventorySystem.Tests.Auth;

public class PasswordHasherTests
{
    private readonly PasswordHasher _passwordHasher = new();

    [Fact]
    public void HashPassword_ReturnsHashedString()
    {
        var password = "SecurePassword123!";
        var hash = _passwordHasher.HashPassword(password);

        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(password);
        hash.Should().StartWith("$2");
    }

    [Fact]
    public void HashPassword_SamePasswordProducesDifferentHashes()
    {
        var password = "SecurePassword123!";
        var hash1 = _passwordHasher.HashPassword(password);
        var hash2 = _passwordHasher.HashPassword(password);

        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
    {
        var password = "SecurePassword123!";
        var hash = _passwordHasher.HashPassword(password);

        _passwordHasher.VerifyPassword(password, hash).Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
    {
        var password = "SecurePassword123!";
        var hash = _passwordHasher.HashPassword(password);

        _passwordHasher.VerifyPassword("WrongPassword456!", hash).Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithEmptyPassword_ReturnsFalse()
    {
        var password = "SecurePassword123!";
        var hash = _passwordHasher.HashPassword(password);

        _passwordHasher.VerifyPassword("", hash).Should().BeFalse();
    }
}
