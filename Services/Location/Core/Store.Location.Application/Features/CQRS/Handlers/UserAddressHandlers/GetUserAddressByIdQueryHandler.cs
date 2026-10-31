using Store.Location.Application.Features.CQRS.Queries.UserAddressQueries;
using Store.Location.Application.Features.CQRS.Results.UserAddressResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.UserAddressHandlers
{
    public class GetUserAddressByIdQueryHandler
    {
        private readonly IRepository<UserAddress> _repository;

        public GetUserAddressByIdQueryHandler(IRepository<UserAddress> repository)
        {
            _repository = repository;
        }

        public async Task<GetUserAddressByIdQueryResult?> Handle(GetUserAddressByIdQuery query)
        {
            var values = await _repository.GetByFilterAsync(x => x.Id == query.Id);
            if (values == null) return null;

            return new GetUserAddressByIdQueryResult
            {
                Id = values.Id,
                Title = values.Title,
                ContactFullName = values.ContactFullName,
                PhoneNumber = values.PhoneNumber,
                AddressDetail = values.AddressDetail,
                CountryId = values.CountryId,
                CityId = values.CityId,
                DistrictId = values.DistrictId,
                NeighborhoodId = values.NeighborhoodId,
                CustomerId = values.CustomerId,
                InvoiceType = values.InvoiceType,
                IdentityNumber = values.IdentityNumber,
                TaxOffice = values.TaxOffice,
                TaxNumber = values.TaxNumber,
                CompanyId = values.CompanyId,
                IsAnonymized = values.IsAnonymized,
                IsActive = values.IsActive,
                IsDefault = values.IsDefault,
                IsDeleted = values.IsDeleted,
                Notes = values.Notes,
                InsertDate = values.InsertDate,
                UpdateDate = values.UpdateDate,
                InsertUserId = values.InsertUserId,
                UpdateUserId = values.UpdateUserId,
                DeleteUserId = values.DeleteUserId,
                DeleteDate = values.DeleteDate,
            };
        }
    }
}
