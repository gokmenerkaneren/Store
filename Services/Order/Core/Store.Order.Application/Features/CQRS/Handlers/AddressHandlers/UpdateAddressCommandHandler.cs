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
    public class UpdateAddressCommandHandler
    {
        private readonly IRepository<Address> _repository;

        public UpdateAddressCommandHandler(IRepository<Address> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateAddressCommand address) {    
            var values = await _repository.GetByIdAsync(address.Id);
            values.UserId = address.UserId;
            values.Country = address.Country;
            values.City = address.City;
            values.District = address.District;
            values.Region = address.Region;
            values.Street = address.Street;
            values.PostalCode = address.PostalCode;
            values.Detail = address.Detail;
            values.Phone = address.Phone;
        }
    }
}
