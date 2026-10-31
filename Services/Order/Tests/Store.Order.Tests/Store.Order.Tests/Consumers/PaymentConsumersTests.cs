using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Store.Order.Application.Interfaces;
using Store.Order.Domain.Entities;
using Store.Order.Domain.Enums;
using Store.Order.WebApi.Consumers;
using Store.Payment.Services;
using Store.Shared.Events;

namespace Store.Order.Tests.Consumers
{
    public class PaymentConsumersTests
    {
        [Fact]
        public async Task PaymentCompletedConsumer_Should_Update_Status_To_Processing_And_Add_Note()
        {
            var ordering = new Ordering { Status = OrderStatus.Pending };
            var repository = CreateRepository(ordering);
            var consumer = new PaymentCompletedConsumer(repository.Object, NullLogger<PaymentCompletedConsumer>.Instance);

            await consumer.Consume(CreateContext(CompletedEvent(ordering.Id)));

            ordering.Status.Should().Be(OrderStatus.Processing);
            ordering.Notes.Should().Contain("Ödeme alındı");
            repository.Verify(r => r.UpdateAsync(ordering), Times.Once);
        }

        // Gerçek akış: StockReservedConsumer siparişi ödeme sonucundan önce Processing yapar.
        [Fact]
        public async Task PaymentCompletedConsumer_Should_Add_Note_Once_When_Order_Is_Already_Processing()
        {
            var ordering = new Ordering { Status = OrderStatus.Processing };
            var repository = CreateRepository(ordering);
            var consumer = new PaymentCompletedConsumer(repository.Object, NullLogger<PaymentCompletedConsumer>.Instance);

            await consumer.Consume(CreateContext(CompletedEvent(ordering.Id)));
            var noteAfterFirstDelivery = ordering.Notes;
            await consumer.Consume(CreateContext(CompletedEvent(ordering.Id)));

            ordering.Status.Should().Be(OrderStatus.Processing);
            noteAfterFirstDelivery.Should().Contain("Ödeme alındı");
            ordering.Notes.Should().Be(noteAfterFirstDelivery);
        }

        [Fact]
        public async Task PaymentFailedConsumer_Should_Cancel_Order_And_Add_Failure_Note()
        {
            var ordering = new Ordering { Status = OrderStatus.Processing, Notes = "Mevcut not" };
            var repository = CreateRepository(ordering);
            var consumer = new PaymentFailedConsumer(repository.Object, NullLogger<PaymentFailedConsumer>.Instance);

            await consumer.Consume(CreateContext(FailedEvent(ordering.Id, "Yetersiz bakiye")));

            ordering.Status.Should().Be(OrderStatus.Cancelled);
            ordering.Notes.Should().Contain("Mevcut not").And.Contain("Ödeme başarısız: Yetersiz bakiye");
            repository.Verify(r => r.UpdateAsync(ordering), Times.Once);
        }

        [Fact]
        public async Task PaymentFailedConsumer_Should_Ignore_When_Order_Cannot_Transition()
        {
            var ordering = new Ordering { Status = OrderStatus.Shipped };
            var repository = CreateRepository(ordering);
            var consumer = new PaymentFailedConsumer(repository.Object, NullLogger<PaymentFailedConsumer>.Instance);

            await consumer.Consume(CreateContext(FailedEvent(ordering.Id, "Yetersiz bakiye")));

            ordering.Status.Should().Be(OrderStatus.Shipped);
            ordering.Notes.Should().BeNullOrEmpty();
            repository.Verify(r => r.UpdateAsync(It.IsAny<Ordering>()), Times.Never);
        }

        [Fact]
        public async Task MockPaymentGateway_Should_Succeed_For_Valid_Card_And_Fail_For_Card_Ending_In_0000()
        {
            var gateway = new MockPaymentGateway();

            var success = await gateway.ProcessPaymentAsync(new PaymentProcessRequest(Guid.NewGuid(), Guid.NewGuid(), 100m, "4111111111111111"));
            var failure = await gateway.ProcessPaymentAsync(new PaymentProcessRequest(Guid.NewGuid(), Guid.NewGuid(), 100m, "4111111111110000"));

            success.IsSuccess.Should().BeTrue();
            success.TransactionId.Should().NotBeNullOrEmpty();
            success.ErrorMessage.Should().BeNull();

            failure.IsSuccess.Should().BeFalse();
            failure.TransactionId.Should().BeNull();
            failure.ErrorMessage.Should().Be("Kart provizyon hatası: Yetersiz bakiye.");
        }

        private static PaymentCompletedIntegrationEvent CompletedEvent(Guid orderId) =>
            new(orderId, Guid.NewGuid(), 100m, DateTime.UtcNow);

        private static PaymentFailedIntegrationEvent FailedEvent(Guid orderId, string reason) =>
            new(orderId, Guid.NewGuid(), 100m, reason, DateTime.UtcNow, new List<OrderItemMessageDto>());

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
