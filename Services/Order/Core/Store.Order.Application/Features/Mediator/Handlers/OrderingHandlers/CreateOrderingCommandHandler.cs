using MediatR;
using Store.Order.Application.Features.Mediator.Commands.OrderingCommands;
using Store.Order.Application.Interfaces;
using Store.Order.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Store.Order.Application.Features.Mediator.Handlers.OrderingHandlers
{
    public class CreateOrderingCommandHandler : IRequestHandler<CreateOrderingCommand, Guid>
    {
        private readonly IRepository<Ordering> _repository;

        public CreateOrderingCommandHandler(IRepository<Ordering> repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateOrderingCommand request, CancellationToken cancellationToken)
        {
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

            await _repository.CreateAsync(ordering);
            return ordering.Id;
        }
    }
}
