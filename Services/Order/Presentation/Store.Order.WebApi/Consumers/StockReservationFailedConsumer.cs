using MassTransit;
using Store.Order.Application.Interfaces;
using Store.Order.Domain.Entities;
using Store.Order.Domain.Enums;
using Store.Shared.Events;

namespace Store.Order.WebApi.Consumers
{
    /// <summary>
    /// Stok ayrılamadığında siparişi iptal eder (kompansasyon) ve sebebi Notes alanına ekler.
    /// Geçiş geçersizse (ör. tekrar teslim edilen mesaj) sipariş olduğu gibi bırakılır.
    /// </summary>
    public class StockReservationFailedConsumer : IConsumer<StockReservationFailedIntegrationEvent>
    {
        // BaseEntity.Notes alanının [StringLength] sınırı
        private const int NotesMaxLength = 1000;

        private readonly IRepository<Ordering> _repository;
        private readonly ILogger<StockReservationFailedConsumer> _logger;

        public StockReservationFailedConsumer(IRepository<Ordering> repository, ILogger<StockReservationFailedConsumer> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<StockReservationFailedIntegrationEvent> context)
        {
            var message = context.Message;
            var ordering = await _repository.GetByIdAsync(message.OrderId);

            if (!ordering.CanTransitionTo(OrderStatus.Cancelled))
            {
                _logger.LogWarning(
                    "Sipariş {OrderId} {Status} durumundan Cancelled durumuna geçirilemedi; stok hatası olayı yok sayıldı. Sebep: {Reason}",
                    ordering.Id,
                    ordering.Status,
                    message.Reason);
                return;
            }

            ordering.ChangeStatus(OrderStatus.Cancelled);

            var cancellationNote = $"Stok rezervasyonu başarısız: {message.Reason}";
            var notes = string.IsNullOrWhiteSpace(ordering.Notes)
                ? cancellationNote
                : $"{ordering.Notes}{Environment.NewLine}{cancellationNote}";
            ordering.Notes = notes.Length > NotesMaxLength ? notes[..NotesMaxLength] : notes;

            await _repository.UpdateAsync(ordering);
        }
    }
}
