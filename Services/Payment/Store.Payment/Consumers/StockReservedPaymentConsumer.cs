using MassTransit;
using Store.Payment.Services;
using Store.Shared.Events;

namespace Store.Payment.Consumers
{
    /// <summary>
    /// Stok ayrıldığında sipariş ödemesini işler ve sonucu PaymentCompleted / PaymentFailed olayı olarak yayınlar.
    /// </summary>
    public class StockReservedPaymentConsumer : IConsumer<StockReservedIntegrationEvent>
    {
        private readonly IPaymentGateway _paymentGateway;
        private readonly ILogger<StockReservedPaymentConsumer> _logger;

        public StockReservedPaymentConsumer(IPaymentGateway paymentGateway, ILogger<StockReservedPaymentConsumer> logger)
        {
            _paymentGateway = paymentGateway;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<StockReservedIntegrationEvent> context)
        {
            var message = context.Message;

            var result = await _paymentGateway.ProcessPaymentAsync(
                new PaymentProcessRequest(message.OrderId, message.UserId, message.TotalPrice),
                context.CancellationToken);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Sipariş {OrderId} ödemesi alındı. İşlem: {TransactionId}", message.OrderId, result.TransactionId);

                await context.Publish(new PaymentCompletedIntegrationEvent(
                    message.OrderId, message.UserId, message.TotalPrice, DateTime.UtcNow));
                return;
            }

            _logger.LogWarning("Sipariş {OrderId} ödemesi reddedildi: {Reason}", message.OrderId, result.ErrorMessage);

            await context.Publish(new PaymentFailedIntegrationEvent(
                message.OrderId, message.UserId, message.TotalPrice, result.ErrorMessage ?? "Ödeme reddedildi.", DateTime.UtcNow, message.Items));
        }
    }
}
