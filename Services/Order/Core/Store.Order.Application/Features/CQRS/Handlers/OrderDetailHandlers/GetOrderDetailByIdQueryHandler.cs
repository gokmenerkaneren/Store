
using Store.Order.Application.Features.CQRS.Queries.OrderDetailQueries;
using Store.Order.Application.Features.CQRS.Results.OrderDetailResults;
using Store.Order.Application.Interfaces;
using Store.Order.Domain.Entities;
using Store.Order.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Order.Application.Features.CQRS.Handlers.OrderDetailHandlers
{
    public class GetOrderDetailByIdQueryHandler
    {
        private readonly IRepository<OrderDetail> _repository;

        public GetOrderDetailByIdQueryHandler(IRepository<OrderDetail> repository)
        {
            _repository = repository;
        }

        public async Task<GetOrderDetailByIdQueryResult> Handle(GetOrderDetailByIdQuery query)
        {
            var  values = await _repository.GetByIdAsync(query.Id);
            return new GetOrderDetailByIdQueryResult
            {
                Id = values.Id,
                OrderingId = values.OrderingId,
                ProductId = values.ProductId,
                ProductPrice = values.ProductPrice,
                ProductAmount = values.ProductAmount,
                TotalPrice = values.TotalPrice,
                PriceTypeId = values.PriceTypeId,
                OriginalPrice = values.OriginalPrice,
                DiscountRateApplied = values.DiscountRateApplied,
                CurrencyId = values.CurrencyId,
                PricingSnapshotDate = values.PricingSnapshotDate                
            };
        }
    }
}

