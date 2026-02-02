using FluentAssertions;
using InventorySystem.Application.DTOs;
using Xunit;

namespace InventorySystem.Tests.Application;

public class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        // Arrange
        var data = "Test Data";

        // Act
        var result = Result<string>.Success(data, "Operación exitosa");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be("Test Data");
        result.Message.Should().Be("Operación exitosa");
    }

    [Fact]
    public void Success_ShouldWorkWithoutMessage()
    {
        // Arrange
        var data = 42;

        // Act
        var result = Result<int>.Success(data);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(42);
        result.Message.Should().BeEmpty();
    }

    [Fact]
    public void Failure_ShouldCreateFailedResult()
    {
        // Act
        var result = Result<string>.Failure("Error de validación");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Message.Should().Be("Error de validación");
    }

    [Fact]
    public void PaginatedResult_ShouldCalculateTotalPages()
    {
        // Arrange
        var items = new List<string> { "item1", "item2" };

        // Act
        var result = new PaginatedResult<string>(items, totalCount: 25, pageNumber: 1, pageSize: 10);

        // Assert
        result.TotalPages.Should().Be(3); // 25 items / 10 per page = 3 pages
        result.TotalCount.Should().Be(25);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public void PaginatedResult_HasPreviousPage_ShouldBeFalse_OnFirstPage()
    {
        // Arrange
        var items = new List<string>();

        // Act
        var result = new PaginatedResult<string>(items, totalCount: 25, pageNumber: 1, pageSize: 10);

        // Assert
        result.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public void PaginatedResult_HasPreviousPage_ShouldBeTrue_OnSecondPage()
    {
        // Arrange
        var items = new List<string>();

        // Act
        var result = new PaginatedResult<string>(items, totalCount: 25, pageNumber: 2, pageSize: 10);

        // Assert
        result.HasPreviousPage.Should().BeTrue();
    }

    [Fact]
    public void PaginatedResult_HasNextPage_ShouldBeTrue_WhenNotOnLastPage()
    {
        // Arrange
        var items = new List<string>();

        // Act
        var result = new PaginatedResult<string>(items, totalCount: 25, pageNumber: 1, pageSize: 10);

        // Assert
        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void PaginatedResult_HasNextPage_ShouldBeFalse_OnLastPage()
    {
        // Arrange
        var items = new List<string>();

        // Act
        var result = new PaginatedResult<string>(items, totalCount: 25, pageNumber: 3, pageSize: 10);

        // Assert
        result.HasNextPage.Should().BeFalse();
    }
}
