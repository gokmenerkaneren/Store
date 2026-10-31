using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Store.Order.Application.Interfaces;
using Store.Order.Domain.Entities;
using Store.Order.Domain.Enums;
using Store.Order.WebApi.Consumers;
using Store.Shared.Events;

namespace Store.Order.Tests.Consumers
{
    public class OrderStockConsumersTests
    {
        [Fact]
        public async Task StockReservedConsumer_Should_Update_Status_To_Processing()
        {
            var ordering = new Ordering { Status = OrderStatus.Pending };
            var repository = CreateRepository(ordering);
            var consumer = new StockReservedConsumer(repository.Object, NullLogger<StockReservedConsumer>.Instance);

            await consumer.Consume(CreateContext(new StockReservedIntegrationEvent(
                ordering.Id, 100m, DateTime.UtcNow, new List<OrderItemMessageDto>())));

            ordering.Status.Should().Be(OrderStatus.Processing);
            repository.Verify(r => r.UpdateAsync(ordering), Times.Once);
        }

        [Fact]
        public async Task StockReservationFailedConsumer_Should_Update_Status_To_Cancelled_And_Record_Reason()
        {
            var ordering = new Ordering { Status = OrderStatus.Pending, Notes = "Mevcut not" };
            var repository = CreateRepository(ordering);
            var consumer = new StockReservationFailedConsumer(repository.Object, NullLogger<StockReservationFailedConsumer>.Instance);

            await consumer.Consume(CreateContext(new StockReservationFailedIntegrationEvent(
                ordering.Id, "Yetersiz stok", DateTime.UtcNow, new List<OrderItemMessageDto>())));

            ordering.Status.Should().Be(OrderStatus.Cancelled);
            ordering.Notes.Should().Contain("Mevcut not").And.Contain("Yetersiz stok");
            repository.Verify(r => r.UpdateAsync(ordering), Times.Once);
        }

        private static Mock<IRepository<Ordering>> CreateRepository(Ordering ordering)
        {
            var repository = new Mock<IRepository<Ordering>>();
            repository.Setup(r => r.GetByIdAsync(ordering.Id)).ReturnsAsync(ordering);
            repository.Setup(r => r.UpdateAsync(It.IsAny<Ordering>())).Returns(Task.CompletedTask);
            return repository;
        }

        private static ConsumeContext<T> CreateContext<T>(T message) where T : class
        {
            var context = new Mock<ConsumeContext<T>>();
            context.Setup(c => c.Message).Returns(message);
            return context.Object;
        }
    }
}
