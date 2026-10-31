using MassTransit;
using Store.Order.Application.Interfaces;
using Store.Order.Domain.Entities;
using Store.Order.Domain.Enums;
using Store.Shared.Events;

namespace Store.Order.WebApi.Consumers
{
    /// <summary>
    /// Stok ayrıldığında siparişi Processing durumuna geçirir.
    /// Geçiş geçersizse (ör. tekrar teslim edilen mesaj) sipariş olduğu gibi bırakılır.
    /// </summary>
    public class StockReservedConsumer : IConsumer<StockReservedIntegrationEvent>
    {
        private readonly IRepository<Ordering> _repository;
        private readonly ILogger<StockReservedConsumer> _logger;

        public StockReservedConsumer(IRepository<Ordering> repository, ILogger<StockReservedConsumer> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<StockReservedIntegrationEvent> context)
        {
            var ordering = await _repository.GetByIdAsync(context.Message.OrderId);

            if (!ordering.CanTransitionTo(OrderStatus.Processing))
            {
                _logger.LogWarning(
                    "Sipariş {OrderId} {Status} durumundan Processing durumuna geçirilemedi; stok ayrıldı olayı yok sayıldı.",
                    ordering.Id,
                    ordering.Status);
                return;
            }

            ordering.ChangeStatus(OrderStatus.Processing);
            await _repository.UpdateAsync(ordering);
        }
    }
}
