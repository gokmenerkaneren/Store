using MassTransit;
using Store.Order.Application.Interfaces;
using Store.Order.Domain.Entities;
using Store.Order.Domain.Enums;
using Store.Shared.Events;

namespace Store.Order.WebApi.Consumers
{
    /// <summary>
    /// Ödeme alındığında siparişi Processing durumuna geçirir ve Notes alanına not ekler.
    /// StockReservedConsumer siparişi genellikle önceden Processing yaptığından
    /// not, durum geçişinden bağımsız olarak eklenir.
    /// </summary>
    public class PaymentCompletedConsumer : IConsumer<PaymentCompletedIntegrationEvent>
    {
        private const string PaymentNote = "Ödeme alındı.";

        // BaseEntity.Notes alanının [StringLength] sınırı
        private const int NotesMaxLength = 1000;

        private readonly IRepository<Ordering> _repository;
        private readonly ILogger<PaymentCompletedConsumer> _logger;

        public PaymentCompletedConsumer(IRepository<Ordering> repository, ILogger<PaymentCompletedConsumer> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentCompletedIntegrationEvent> context)
        {
            var ordering = await _repository.GetByIdAsync(context.Message.OrderId);

            if (ordering.CanTransitionTo(OrderStatus.Processing))
            {
                ordering.ChangeStatus(OrderStatus.Processing);
            }
            else if (ordering.Status != OrderStatus.Processing)
            {
                _logger.LogWarning(
                    "Sipariş {OrderId} {Status} durumundayken ödeme alındı; durum değiştirilmedi.",
                    ordering.Id,
                    ordering.Status);
            }

            // Tekrar teslim edilen mesaj notu çoğaltmasın.
            if (ordering.Notes?.Contains(PaymentNote) != true)
            {
                var notes = string.IsNullOrWhiteSpace(ordering.Notes)
                    ? PaymentNote
                    : $"{ordering.Notes}{Environment.NewLine}{PaymentNote}";
                ordering.Notes = notes.Length > NotesMaxLength ? notes[..NotesMaxLength] : notes;
            }

            await _repository.UpdateAsync(ordering);
        }
    }
}
