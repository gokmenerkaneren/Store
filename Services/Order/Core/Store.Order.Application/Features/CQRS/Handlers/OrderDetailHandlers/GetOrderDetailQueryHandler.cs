using Store.Order.Application.Features.CQRS.Results.OrderDetailResults;
using Store.Order.Application.Interfaces;
using Store.Order.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Order.Application.Features.CQRS.Handlers.OrderDetailHandlers
{
    public class GetOrderDetailQueryHandler
    {
        private readonly IRepository<OrderDetail> _repository;
        public GetOrderDetailQueryHandler(IRepository<OrderDetail> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetOrderDetailQueryResult>> Handle()
        {
            var values = await _repository.GetAllAsync();
            return values.Select(x => new GetOrderDetailQueryResult
            {
                Id = x.Id,
                OrderingId = x.OrderingId,
                ProductId = x.ProductId,
                ProductAmount = x.ProductAmount,
                ProductPrice = x.ProductPrice,
                TotalPrice = x.TotalPrice,
                PriceTypeId = x.PriceTypeId,
                OriginalPrice = x.OriginalPrice,
                DiscountRateApplied = x.DiscountRateApplied,
                CurrencyId = x.CurrencyId,
                PricingSnapshotDate = x.PricingSnapshotDate
            }).ToList();
        }
    }
}

