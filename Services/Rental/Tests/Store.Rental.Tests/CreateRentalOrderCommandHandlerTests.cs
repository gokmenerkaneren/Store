using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Store.Rental.Application.Features.RentalOrders.Commands.CreateRentalOrder;
using Store.Rental.Domain.Entities;
using Store.Rental.Domain.Enums;
using Store.Rental.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Store.Rental.Tests
{
    public class CreateRentalOrderCommandHandlerTests
    {
        private RentalDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<RentalDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new RentalDbContext(options);
        }

        [Fact]
        public async Task Handle_Should_Calculate_TotalAmount_Multiplying_Quantity_UnitPrice_And_DurationDays()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var handler = new CreateRentalOrderCommandHandler(context);

            var userId = Guid.NewGuid();
            var startDate = new DateTime(2026, 11, 1);
            var endDate = new DateTime(2026, 11, 4); // 3 gün
            var expectedDays = 3;
            var quantity = 2;
            var unitPrice = 150m;
            var expectedTotal = quantity * unitPrice * expectedDays; // 2 * 150 * 3 = 900

            var command = new CreateRentalOrderCommand
            {
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate,
                Items = new List<RentalOrderItemDto>
                {
                    new RentalOrderItemDto { ProductId = Guid.NewGuid().ToString(), Quantity = quantity, UnitPrice = unitPrice }
                }
            };

            // Act
            var orderId = await handler.Handle(command, CancellationToken.None);

            // Assert
            var savedOrder = await context.RentalOrders.Include(o => o.RentalOrderItems).FirstOrDefaultAsync(o => o.Id == orderId);
            savedOrder.Should().NotBeNull();
            savedOrder.TotalAmount.Should().Be(expectedTotal);
            savedOrder.RentalOrderItems.Should().HaveCount(1);
            var firstItem = savedOrder.RentalOrderItems.First();
            firstItem.Quantity.Should().Be(quantity);
            firstItem.UnitPrice.Should().Be(unitPrice);
        }

        [Fact]
        public async Task Handle_Should_Throw_InvalidOperationException_When_EndDate_Is_Before_Or_Equal_StartDate()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var handler = new CreateRentalOrderCommandHandler(context);

            var command = new CreateRentalOrderCommand
            {
                UserId = Guid.NewGuid(),
                StartDate = new DateTime(2026, 11, 5),
                EndDate = new DateTime(2026, 11, 3), // Geçersiz aralık
                Items = new List<RentalOrderItemDto>
                {
                    new RentalOrderItemDto { ProductId = "p1", Quantity = 1, UnitPrice = 100 }
                }
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_Should_Throw_InvalidOperationException_When_Item_UnitPrice_Is_Zero_Or_Negative()
        {
            // Arrange: Fiyat manipülasyonu denemesi (0 veya eksi fiyat)
            using var context = CreateInMemoryDbContext();
            var handler = new CreateRentalOrderCommandHandler(context);

            var command = new CreateRentalOrderCommand
            {
                UserId = Guid.NewGuid(),
                StartDate = new DateTime(2026, 11, 1),
                EndDate = new DateTime(2026, 11, 2),
                Items = new List<RentalOrderItemDto>
                {
                    new RentalOrderItemDto { ProductId = "p1", Quantity = 1, UnitPrice = 0 }
                }
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
            ex.Message.Should().Contain("fiyatı 0'dan büyük olmalıdır");
        }

        [Fact]
        public async Task Handle_Should_Throw_InvalidOperationException_When_Item_Quantity_Is_Zero_Or_Negative()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var handler = new CreateRentalOrderCommandHandler(context);

            var command = new CreateRentalOrderCommand
            {
                UserId = Guid.NewGuid(),
                StartDate = new DateTime(2026, 11, 1),
                EndDate = new DateTime(2026, 11, 2),
                Items = new List<RentalOrderItemDto>
                {
                    new RentalOrderItemDto { ProductId = "p1", Quantity = 0, UnitPrice = 50 }
                }
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
            ex.Message.Should().Contain("miktarı 0'dan büyük olmalıdır");
        }

        [Fact]
        public async Task Handle_Should_Throw_Exception_When_Date_Conflict_Detected()
        {
            // Arrange: Çakışan mevcut teslim edilmiş sipariş var
            using var context = CreateInMemoryDbContext();
            var productId = Guid.NewGuid().ToString();

            var existingOrder = new RentalOrder
            {
                UserId = Guid.NewGuid(),
                StartDate = new DateTime(2026, 11, 10),
                EndDate = new DateTime(2026, 11, 15),
                Status = RentalStatus.Approved
            };
            var existingItem = new RentalOrderItem
            {
                ProductId = productId,
                Quantity = 1,
                UnitPrice = 100,
                RentalOrder = existingOrder
            };
            context.RentalOrders.Add(existingOrder);
            context.RentalOrderItems.Add(existingItem);
            await context.SaveChangesAsync();

            var handler = new CreateRentalOrderCommandHandler(context);

            var overlappingCommand = new CreateRentalOrderCommand
            {
                UserId = Guid.NewGuid(),
                StartDate = new DateTime(2026, 11, 12),
                EndDate = new DateTime(2026, 11, 18), // Çakışıyor
                Items = new List<RentalOrderItemDto>
                {
                    new RentalOrderItemDto { ProductId = productId, Quantity = 1, UnitPrice = 100 }
                }
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(overlappingCommand, CancellationToken.None));
        }
    }
}
