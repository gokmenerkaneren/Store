using Store.Order.Application.Features.CQRS.Queries.AddressQueries;
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
    public class GetAddressByIdQueryHandler
    {
        private readonly IRepository<Address> _repository;

        public GetAddressByIdQueryHandler(IRepository<Address> repository)
        {
            _repository = repository;
        }

        public async Task<GetAddressByIdQueryResult> Handle(GetAddressByIdQuery query)
        {
            var address = await _repository.GetByIdAsync(query.Id);
            return new GetAddressByIdQueryResult
            {
                Id = address.Id,
                UserId = address.UserId,
                Country = address.Country,
                City = address.City,
                District = address.District,
                Region = address.Region,
                Street = address.Street,
                PostalCode = address.PostalCode,
                Detail = address.Detail,
                Phone = address.Phone
            };
        }
    }
}
