using AutoMapper;
using FluentAssertions;
using Moq;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.Common.Mappings;
using InventorySystem.Application.Features.Products.Commands;
using InventorySystem.Domain.Entities;
using InventorySystem.Domain.Interfaces;
using Xunit;

namespace InventorySystem.Tests.Products;

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
        
        _handler = new CreateProductCommandHandler(_unitOfWorkMock.Object, _mapper, _notificationServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidProduct_ReturnsSuccess()
    {
        var command = new CreateProductCommand("Producto Test", "Descripción test", 100.00m, 10, null, "SKU-001", null);

        _unitOfWorkMock.Setup(x => x.Products.ExistsByNameAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _unitOfWorkMock.Setup(x => x.Products.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken _) => p);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Producto Test");
    }

    [Fact]
    public async Task Handle_NegativeQuantity_ReturnsFailure()
    {
        var command = new CreateProductCommand("Producto Test", "Descripción", 100.00m, -5, null, "SKU-001", null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("negativa");
    }

    [Fact]
    public async Task Handle_DuplicateName_ReturnsFailure()
    {
        var command = new CreateProductCommand("Producto Existente", "Descripción", 100.00m, 10, null, "SKU-001", null);

        _unitOfWorkMock.Setup(x => x.Products.ExistsByNameAsync(command.Name, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("Ya existe");
    }

    [Fact]
    public async Task Handle_LowStockProduct_SendsNotification()
    {
        var command = new CreateProductCommand("Producto Bajo Stock", "Descripción", 50.00m, 3, null, "SKU-LOW", null);

        _unitOfWorkMock.Setup(x => x.Products.ExistsByNameAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _unitOfWorkMock.Setup(x => x.Products.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken _) => p);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _notificationServiceMock.Verify(x => x.NotifyLowStockAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
