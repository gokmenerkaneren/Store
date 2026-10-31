using Store.Order.Application.Features.CQRS.Results.AddressResults;
using Store.Order.Application.Interfaces;
using Store.Order.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Order.Application.Features.CQRS.Handlers.AddressHandlers
{
    public class GetAddressQueryHandler
    {
        private readonly IRepository<Address> _repository;

        public GetAddressQueryHandler(IRepository<Address> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetAddressQueryResult>> Handle()
        {
            var addresses = await _repository.GetAllAsync();
            return addresses.Select(x => new GetAddressQueryResult
            {
                Id = x.Id,
                UserId = x.UserId,
                Country = x.Country,
                City = x.City,
                District = x.District,
                Region = x.Region,
                Street = x.Street,
                PostalCode = x.PostalCode,
                Detail = x.Detail,
                Phone = x.Phone
            }).ToList();
        }
    }
}
