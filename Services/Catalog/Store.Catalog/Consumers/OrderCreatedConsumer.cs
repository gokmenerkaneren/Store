using MassTransit;
using Store.Shared.Events;

namespace Store.Catalog.Consumers
{
    /// <summary>
    /// Sipariş oluşturuldu olayını tüketir. Şimdilik iskelet: stok düşümü henüz uygulanmadı.
    /// </summary>
    public class OrderCreatedConsumer : IConsumer<OrderCreatedIntegrationEvent>
    {
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "Sipariş alındı: {OrderId}, {ItemCount} kalem için stok düşüm hazırlığı yapıldı.",
                message.OrderId,
                message.Items.Count);

            return Task.CompletedTask;
        }
    }
}
