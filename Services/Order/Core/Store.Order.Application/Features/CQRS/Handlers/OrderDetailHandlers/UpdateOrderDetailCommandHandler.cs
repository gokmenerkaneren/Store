using Store.Order.Application.Features.CQRS.Commands.AddressCommands;
using Store.Order.Application.Features.CQRS.Commands.OrderDetailCommands;
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
    public class UpdateOrderDetailCommandHandler
    {
        private readonly IRepository<OrderDetail> _repository;

        public UpdateOrderDetailCommandHandler(IRepository<OrderDetail> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateOrderDetailCommand command)
        {
            var values = await _repository.GetByIdAsync(command.Id);
            values.Id = command.Id;
            values.OrderingId = command.OrderingId;
            values.ProductId = command.ProductId;
            values.ProductPrice = command.ProductPrice;
            values.ProductAmount = command.ProductAmount;
            values.TotalPrice = command.TotalPrice;
            values.PriceTypeId = command.PriceTypeId;
            values.OriginalPrice = command.OriginalPrice;
            values.DiscountRateApplied = command.DiscountRateApplied;
            values.CurrencyId = command.CurrencyId;
            values.PricingSnapshotDate = command.PricingSnapshotDate;
            await _repository.UpdateAsync(values);
        }
    }
}

