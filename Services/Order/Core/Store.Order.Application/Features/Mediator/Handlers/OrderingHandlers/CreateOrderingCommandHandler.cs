using MassTransit;
using MediatR;
using Store.Order.Application.Features.Mediator.Commands.OrderingCommands;
using Store.Order.Application.Interfaces;
using Store.Order.Domain.Entities;
using Store.Shared.Events;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Store.Order.Application.Features.Mediator.Handlers.OrderingHandlers
{
    public class CreateOrderingCommandHandler : IRequestHandler<CreateOrderingCommand, Guid>
    {
        private readonly IRepository<Ordering> _repository;
        private readonly IStockValidationService? _stockValidationService;
        private readonly IPublishEndpoint? _publishEndpoint;

        public CreateOrderingCommandHandler(
            IRepository<Ordering> repository,
            IStockValidationService? stockValidationService = null,
            IPublishEndpoint? publishEndpoint = null)
        {
            _repository = repository;
            _stockValidationService = stockValidationService;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Guid> Handle(CreateOrderingCommand request, CancellationToken cancellationToken)
        {
            // İ6: Stok ve miktar doğrulama (Stock & Quantity Validation)
            if (request.Items != null && request.Items.Any())
            {
                foreach (var item in request.Items)
                {
                    if (item.ProductAmount <= 0)
                    {
                        throw new InvalidOperationException($"Ürün adedi 0 veya negatif olamaz (ProductId: {item.ProductId}).");
                    }

                    if (_stockValidationService != null)
                    {
                        var hasStock = await _stockValidationService.ValidateStockAsync(item.ProductId, item.ProductAmount, cancellationToken);
                        if (!hasStock)
                        {
                            throw new InvalidOperationException($"Yetersiz stok: {item.ProductId} numaralı ürün için {item.ProductAmount} adet stok mevcut değil.");
                        }
                    }
                }
            }

            var ordering = new Ordering
            {
                UserId = request.UserId,
                TotalPrice = request.TotalPrice,
                OrderDate = request.OrderDate,
                Status = request.Status,
                InsertUserId = request.UserId
            };

            // İ2: Sipariş kalemlerinin (OrderDetail) sipariş ile birlikte atomik olarak kaydedilmesi
            if (request.Items != null && request.Items.Any())
            {
                foreach (var item in request.Items)
                {
                    var calculatedItemTotal = item.TotalPrice > 0 
                        ? item.TotalPrice 
                        : (item.ProductPrice * item.ProductAmount);

                    ordering.OrderDetails.Add(new OrderDetail
                    {
                        ProductId = item.ProductId,
                        ProductPrice = item.ProductPrice,
                        ProductAmount = item.ProductAmount,
                        TotalPrice = calculatedItemTotal,
                        PriceTypeId = item.PriceTypeId,
                        OriginalPrice = item.OriginalPrice > 0 ? item.OriginalPrice : item.ProductPrice,
                        DiscountRateApplied = item.DiscountRateApplied,
                        CurrencyId = item.CurrencyId,
                        PricingSnapshotDate = DateTimeOffset.UtcNow,
                        InsertUserId = request.UserId
                    });
                }
            }

            // Bus Outbox: Publish mesajı yalnızca DbContext'e ekler; asıl yazma SaveChanges ile olur.
            // Repository.CreateAsync SaveChanges çağırdığı için Publish ondan ÖNCE yapılmalıdır;
            // böylece sipariş ve olay aynı transaction'da kaydedilir. Sonrasında yapılsaydı olay kaybolurdu.
            if (_publishEndpoint != null)
            {
                var integrationEvent = new OrderCreatedIntegrationEvent(
                    ordering.Id,
                    ordering.UserId,
                    ordering.TotalPrice,
                    ordering.OrderDate,
                    ordering.OrderDetails
                        .Select(d => new OrderItemMessageDto(d.ProductId, d.ProductAmount, d.ProductPrice))
                        .ToList());

                await _publishEndpoint.Publish(integrationEvent, cancellationToken);
            }

            await _repository.CreateAsync(ordering);
            return ordering.Id;
        }
    }
}
