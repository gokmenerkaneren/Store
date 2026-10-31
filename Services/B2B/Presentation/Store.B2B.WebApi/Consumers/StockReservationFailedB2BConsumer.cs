using MassTransit;
using Microsoft.EntityFrameworkCore;
using Store.B2B.Persistence.Context;
using Store.Shared.Entities;
using Store.Shared.Events;

namespace Store.B2B.WebApi.Consumers
{
    /// <summary>
    /// Stok ayrılamadığında sipariş tutarını bayinin cari bakiyesinden geri düşer (kompansasyon).
    /// </summary>
    public class StockReservationFailedB2BConsumer : IConsumer<StockReservationFailedIntegrationEvent>
    {
        private readonly B2BContext _context;
        private readonly ILogger<StockReservationFailedB2BConsumer> _logger;

        public StockReservationFailedB2BConsumer(B2BContext context, ILogger<StockReservationFailedB2BConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<StockReservationFailedIntegrationEvent> context)
        {
            var message = context.Message;
            var cancellationToken = context.CancellationToken;

            var dealer = await _context.DealerFinancials
                .FirstOrDefaultAsync(x => x.DealerId == message.UserId, cancellationToken);

            if (dealer == null)
            {
                return;
            }

            dealer.CurrentBalance = Math.Max(0m, dealer.CurrentBalance - message.TotalPrice);
            dealer.UpdateDate = BaseEntity.GetLocalTime();
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Stok ayrılamadı: bayi {DealerId} bakiyesinden sipariş {OrderId} tutarı ({TotalPrice}) geri alındı.",
                message.UserId,
                message.OrderId,
                message.TotalPrice);
        }
    }
}
