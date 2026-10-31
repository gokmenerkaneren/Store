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
    public class RemoveOrderDetailCommandHandler
    {
        private readonly IRepository<OrderDetail> _repository;

        public RemoveOrderDetailCommandHandler(IRepository<OrderDetail> repository)
        {
            _repository = repository;
        }
        public async Task Handle(RemoveOrderDetailCommand command)
        {
            var orderDetail = await _repository.GetByIdAsync(command.Id);
            if (orderDetail != null)
            {
                await _repository.SoftDeleteAsync(orderDetail);
            }
        }
    }
}

