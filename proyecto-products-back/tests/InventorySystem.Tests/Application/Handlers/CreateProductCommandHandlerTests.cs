using AutoMapper;
using FluentAssertions;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.Common.Mappings;
using InventorySystem.Application.DTOs;
using InventorySystem.Application.Features.Products.Commands;
using InventorySystem.Domain.Entities;
using InventorySystem.Domain.Interfaces;
using Moq;
using Xunit;

namespace InventorySystem.Tests.Application.Handlers;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly IMapper _mapper;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _notificationServiceMock = new Mock<INotificationService>();
        
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _handler = new CreateProductCommandHandler(
            _unitOfWorkMock.Object,
            _mapper,
            _notificationServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldCreateProduct_WhenValidCommand()
    {
        // Arrange
        var command = new CreateProductCommand(
            Name: "Test Product",
            Description: "Description",
            Price: 100m,
            StockQuantity: 10,
            CategoryId: null,
            Sku: "TEST-001",
            ImageUrl: null
        );

        _unitOfWorkMock.Setup(x => x.Products.ExistsByNameAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock.Setup(x => x.Products.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Test Product");
        result.Data.Price.Should().Be(100m);
        result.Data.StockQuantity.Should().Be(10);

        _unitOfWorkMock.Verify(x => x.Products.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenProductNameAlreadyExists()
    {
        // Arrange
        var command = new CreateProductCommand(
            Name: "Existing Product",
            Description: null,
            Price: 100m,
            StockQuantity: 10,
            CategoryId: null,
            Sku: null,
            ImageUrl: null
        );

        _unitOfWorkMock.Setup(x => x.Products.ExistsByNameAsync("Existing Product", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("Ya existe un producto");

        _unitOfWorkMock.Verify(x => x.Products.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenStockIsNegative()
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
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("no puede ser negativa");
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCategoryDoesNotExist()
    {
        // Arrange
        var command = new CreateProductCommand(
            Name: "Test Product",
            Description: null,
            Price: 100m,
            StockQuantity: 10,
            CategoryId: "non-existent-category",
            Sku: null,
            ImageUrl: null
        );

        _unitOfWorkMock.Setup(x => x.Products.ExistsByNameAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock.Setup(x => x.Categories.GetByIdAsync("non-existent-category", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("categoría");
    }

    [Fact]
    public async Task Handle_ShouldNotifyLowStock_WhenStockIsBelowThreshold()
    {
        // Arrange
        var command = new CreateProductCommand(
            Name: "Low Stock Product",
            Description: null,
            Price: 100m,
            StockQuantity: 3, // Below threshold (5)
            CategoryId: null,
            Sku: null,
            ImageUrl: null
        );

        _unitOfWorkMock.Setup(x => x.Products.ExistsByNameAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock.Setup(x => x.Products.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _notificationServiceMock.Setup(x => x.NotifyLowStockAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        _notificationServiceMock.Verify(
            x => x.NotifyLowStockAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), 
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ShouldNotNotifyLowStock_WhenStockIsAboveThreshold()
    {
        // Arrange
        var command = new CreateProductCommand(
            Name: "Normal Stock Product",
            Description: null,
            Price: 100m,
            StockQuantity: 10, // Above threshold (5)
            CategoryId: null,
            Sku: null,
            ImageUrl: null
        );

        _unitOfWorkMock.Setup(x => x.Products.ExistsByNameAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock.Setup(x => x.Products.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        _notificationServiceMock.Verify(
            x => x.NotifyLowStockAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), 
            Times.Never
        );
    }
}
