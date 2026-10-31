using Store.Order.Application.Features.CQRS.Commands.AddressCommands;
using Store.Order.Application.Interfaces;
using Store.Order.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Order.Application.Features.CQRS.Handlers.AddressHandlers
{
    public class RemoveAddressCommandHandler
    {
        private readonly IRepository<Address> _repository;

        public RemoveAddressCommandHandler(IRepository<Address> repository)
        {
            _repository = repository;
        }
        public async Task Handle(RemoveAddressCommand command)
        {
            var address = await _repository.GetByIdAsync(command.Id);
            if (address != null)
            {
                await _repository.SoftDeleteAsync(address);
            }
        }
    }
}
