using Store.Order.Application.Features.CQRS.Commands.OrderDetailCommands;
using Store.Order.Application.Interfaces;
using Store.Order.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Order.Application.Features.CQRS.Handlers.OrderDetailHandlers
{
    public class CreateOrderDetailCommandHandler
    {
        private readonly IRepository<OrderDetail> _repository;
        public CreateOrderDetailCommandHandler(IRepository<OrderDetail> repository)
        {
            _repository = repository;
        }
        
        public async Task Handle(CreateOrderDetailCommand request)
        {
            var orderDetail = new OrderDetail
            {
                OrderingId = request.OrderingId,
                ProductId = request.ProductId,
                ProductAmount = request.ProductAmount,
                ProductPrice = request.ProductPrice,
                TotalPrice = request.TotalPrice,
                PriceTypeId = request.PriceTypeId,
                OriginalPrice = request.OriginalPrice,
                DiscountRateApplied = request.DiscountRateApplied,
                CurrencyId = request.CurrencyId,
                PricingSnapshotDate = request.PricingSnapshotDate
            };
            await _repository.CreateAsync(orderDetail);
        }


    }
}

