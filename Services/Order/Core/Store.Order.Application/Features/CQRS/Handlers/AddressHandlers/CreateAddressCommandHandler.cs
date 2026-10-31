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
    public class CreateAddressCommandHandler
    {
        private readonly IRepository<Address> _repository;

        public CreateAddressCommandHandler(IRepository<Address> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateAddressCommand createAddressCommand)
        {
            var address = new Address
            {
                UserId = createAddressCommand.UserId,
                Country = createAddressCommand.Country,
                City = createAddressCommand.City,
                District = createAddressCommand.District,
                Region = createAddressCommand.Region,
                Street = createAddressCommand.Street,
                PostalCode = createAddressCommand.PostalCode,
                Detail = createAddressCommand.Detail,
                Phone = createAddressCommand.Phone
            };
            await _repository.CreateAsync(address);
        }
    }
}
