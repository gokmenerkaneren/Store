using FluentAssertions;
using Moq;
using Store.Order.Application.Features.Mediator.Commands.OrderingCommands;
using Store.Order.Application.Features.Mediator.Handlers.OrderingHandlers;
using Store.Order.Application.Interfaces;
using Store.Order.Domain.Entities;
using Store.Order.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Store.Order.Tests.Handlers
{
    public class CreateOrderingCommandHandlerTests
    {
        private readonly Mock<IRepository<Ordering>> _repositoryMock;
        private readonly CreateOrderingCommandHandler _handler;

        public CreateOrderingCommandHandlerTests()
        {
            _repositoryMock = new Mock<IRepository<Ordering>>();
            _handler = new CreateOrderingCommandHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Save_Ordering_With_Status_Pending_Default_State_Machine()
        {
            // Arrange (İ3 Durum Makinesi Testi)
            var userId = Guid.NewGuid();
            var command = new CreateOrderingCommand
            {
                UserId = userId,
                TotalPrice = 350,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending
            };

            Ordering capturedOrdering = null;
            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Ordering>()))
                           .Callback<Ordering>(o => capturedOrdering = o)
                           .Returns(Task.CompletedTask);

            // Act
            var orderId = await _handler.Handle(command, CancellationToken.None);

            // Assert
            orderId.Should().NotBeEmpty();
            capturedOrdering.Should().NotBeNull();
            capturedOrdering.Status.Should().Be(OrderStatus.Pending);
            capturedOrdering.UserId.Should().Be(userId);
            capturedOrdering.TotalPrice.Should().Be(350);
        }

        [Fact]
        public async Task Handle_Should_Save_Ordering_With_OrderDetails_When_Items_Provided()
        {
            // Arrange (İ2 Sipariş Kalemlerinin Kaydedilmesi Testi)
            var userId = Guid.NewGuid();
            var prod1 = Guid.NewGuid();
            var prod2 = Guid.NewGuid();

            var command = new CreateOrderingCommand
            {
                UserId = userId,
                TotalPrice = 500,
                OrderDate = DateTime.UtcNow,
                Items = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto { ProductId = prod1, ProductPrice = 100, ProductAmount = 2, TotalPrice = 200 },
                    new CreateOrderItemDto { ProductId = prod2, ProductPrice = 300, ProductAmount = 1, TotalPrice = 300 }
                }
            };

            Ordering capturedOrdering = null;
            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Ordering>()))
                           .Callback<Ordering>(o => capturedOrdering = o)
                           .Returns(Task.CompletedTask);

            // Act
            var orderId = await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedOrdering.Should().NotBeNull();
            capturedOrdering.OrderDetails.Should().HaveCount(2);

            var item1 = capturedOrdering.OrderDetails.First(x => x.ProductId == prod1);
            item1.ProductAmount.Should().Be(2);
            item1.ProductPrice.Should().Be(100);
            item1.TotalPrice.Should().Be(200);
            item1.InsertUserId.Should().Be(userId);

            var item2 = capturedOrdering.OrderDetails.First(x => x.ProductId == prod2);
            item2.ProductAmount.Should().Be(1);
            item2.ProductPrice.Should().Be(300);
            item2.TotalPrice.Should().Be(300);
        }

        [Fact]
        public async Task Handle_Should_Calculate_Item_TotalPrice_If_Not_Explicitly_Provided()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var prodId = Guid.NewGuid();

            var command = new CreateOrderingCommand
            {
                UserId = userId,
                TotalPrice = 150,
                OrderDate = DateTime.UtcNow,
                Items = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto { ProductId = prodId, ProductPrice = 75, ProductAmount = 2, TotalPrice = 0 }
                }
            };

            Ordering capturedOrdering = null;
            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Ordering>()))
                           .Callback<Ordering>(o => capturedOrdering = o)
                           .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert: TotalPrice otomatik olarak 75 * 2 = 150 hesaplanmalı
            capturedOrdering.Should().NotBeNull();
            capturedOrdering.OrderDetails.Should().HaveCount(1);
            capturedOrdering.OrderDetails[0].TotalPrice.Should().Be(150);
        }

        [Fact]
        public async Task Handle_Should_Throw_InvalidOperationException_When_Item_ProductAmount_Is_Zero_Or_Negative()
        {
            // Arrange (İ6 Miktar Güvenlik Doğrulaması)
            var command = new CreateOrderingCommand
            {
                UserId = Guid.NewGuid(),
                TotalPrice = 100,
                OrderDate = DateTime.UtcNow,
                Items = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto { ProductId = Guid.NewGuid(), ProductPrice = 50, ProductAmount = 0 }
                }
            };

            // Act & Assert
            var act = () => _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("*Ürün adedi 0 veya negatif olamaz*");
        }

        [Fact]
        public async Task Handle_Should_Throw_InvalidOperationException_When_Stock_Is_Insufficient()
        {
            // Arrange (İ6 Temel Stok Doğrulama - Yetersiz Stok Koruması)
            var stockServiceMock = new Mock<IStockValidationService>();
            var prodId = Guid.NewGuid();
            stockServiceMock.Setup(s => s.ValidateStockAsync(prodId, 5, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(false);

            var handlerWithStock = new CreateOrderingCommandHandler(_repositoryMock.Object, stockServiceMock.Object);

            var command = new CreateOrderingCommand
            {
                UserId = Guid.NewGuid(),
                TotalPrice = 500,
                OrderDate = DateTime.UtcNow,
                Items = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto { ProductId = prodId, ProductPrice = 100, ProductAmount = 5 }
                }
            };

            // Act & Assert
            var act = () => handlerWithStock.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("*Yetersiz stok*");
        }

        [Fact]
        public async Task Handle_Should_Succeed_When_Stock_Is_Sufficient()
        {
            // Arrange (İ6 Temel Stok Doğrulama - Yeterli Stok Başarı Akışı)
            var stockServiceMock = new Mock<IStockValidationService>();
            var prodId = Guid.NewGuid();
            stockServiceMock.Setup(s => s.ValidateStockAsync(prodId, 2, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(true);

            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Ordering>()))
                           .Returns(Task.CompletedTask);

            var handlerWithStock = new CreateOrderingCommandHandler(_repositoryMock.Object, stockServiceMock.Object);

            var command = new CreateOrderingCommand
            {
                UserId = Guid.NewGuid(),
                TotalPrice = 200,
                OrderDate = DateTime.UtcNow,
                Items = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto { ProductId = prodId, ProductPrice = 100, ProductAmount = 2 }
                }
            };

            // Act
            var orderId = await handlerWithStock.Handle(command, CancellationToken.None);

            // Assert
            orderId.Should().NotBeEmpty();
            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Ordering>()), Times.Once);
        }
    }
}
