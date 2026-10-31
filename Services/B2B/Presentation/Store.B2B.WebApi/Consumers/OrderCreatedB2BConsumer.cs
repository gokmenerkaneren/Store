using MassTransit;
using Microsoft.EntityFrameworkCore;
using Store.B2B.Persistence.Context;
using Store.Shared.Entities;
using Store.Shared.Events;

namespace Store.B2B.WebApi.Consumers
{
    /// <summary>
    /// Sipariş oluşturuldu olayını tüketir: sipariş bir bayiye aitse tutarı bayinin cari bakiyesine ekler.
    /// </summary>
    public class OrderCreatedB2BConsumer : IConsumer<OrderCreatedIntegrationEvent>
    {
        private readonly B2BContext _context;
        private readonly ILogger<OrderCreatedB2BConsumer> _logger;

        public OrderCreatedB2BConsumer(B2BContext context, ILogger<OrderCreatedB2BConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
        {
            var message = context.Message;
            var cancellationToken = context.CancellationToken;

            var dealerFinancial = await _context.DealerFinancials
                .FirstOrDefaultAsync(x => x.DealerId == message.UserId, cancellationToken);

            if (dealerFinancial == null)
            {
                _logger.LogInformation(
                    "Sipariş {OrderId} bir B2C müşterisine ait, bayi bakiyesi güncellenmedi. Kullanıcı: {UserId}",
                    message.OrderId,
                    message.UserId);
                return;
            }

            dealerFinancial.CurrentBalance += message.TotalPrice;
            dealerFinancial.UpdateDate = BaseEntity.GetLocalTime();

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Bayi {DealerId} bakiyesi güncellenirken eşzamanlılık çakışması oluştu. Sipariş: {OrderId}",
                    message.UserId,
                    message.OrderId);
                throw;
            }

            _logger.LogInformation(
                "Bayi {DealerId} bakiyesine sipariş {OrderId} tutarı ({TotalPrice}) eklendi.",
                message.UserId,
                message.OrderId,
                message.TotalPrice);
        }
    }
}
