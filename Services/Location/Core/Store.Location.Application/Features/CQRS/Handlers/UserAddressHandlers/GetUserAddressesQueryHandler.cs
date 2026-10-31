using Store.Location.Application.Features.CQRS.Results.UserAddressResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.UserAddressHandlers
{
    public class GetUserAddressesQueryHandler
    {
        private readonly IRepository<UserAddress> _repository;

        public GetUserAddressesQueryHandler(IRepository<UserAddress> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetUserAddressQueryResult>> Handle(bool showDeleted = false)
        {
            var userAddresses = await _repository.GetAllAsync(includeDeleted: showDeleted);
            if (!showDeleted) userAddresses = userAddresses.Where(x => !x.IsDeleted).ToList();

            // Müşteri bazında tek adresi olanlar için IsDefault güvencesi
            var customerGroups = userAddresses.GroupBy(x => x.CustomerId);
            foreach (var group in customerGroups)
            {
                var activeList = group.ToList();
                if (activeList.Count == 1 && !activeList[0].IsDefault)
                {
                    activeList[0].IsDefault = true;
                }
                else if (activeList.Count > 1 && !activeList.Any(x => x.IsDefault))
                {
                    // Birden fazla adresi olup hiçbiri varsayılan değilse, en yeniyi varsayılan yap
                    var latest = activeList.OrderByDescending(x => x.InsertDate).First();
                    latest.IsDefault = true;
                }
            }

            return userAddresses
                .OrderByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.InsertDate)
                .Select(x => new GetUserAddressQueryResult
                {
                    Id = x.Id,
                    Title = x.Title,
                    ContactFullName = x.ContactFullName,
                    PhoneNumber = x.PhoneNumber,
                    AddressDetail = x.AddressDetail,
                    CountryId = x.CountryId,
                    CityId = x.CityId,
                    DistrictId = x.DistrictId,
                    NeighborhoodId = x.NeighborhoodId,
                    CustomerId = x.CustomerId,
                    InvoiceType = x.InvoiceType,
                    IdentityNumber = x.IdentityNumber,
                    TaxOffice = x.TaxOffice,
                    TaxNumber = x.TaxNumber,
                    CompanyId = x.CompanyId,
                    IsAnonymized = x.IsAnonymized,
                    IsActive = x.IsActive,
                    IsDefault = x.IsDefault,
                    Notes = x.Notes,
                    InsertUserId = x.InsertUserId,
                    InsertDate = x.InsertDate,
                    UpdateUserId = x.UpdateUserId,
                    UpdateDate = x.UpdateDate,
                    IsDeleted = x.IsDeleted,
                    DeleteUserId = x.DeleteUserId,
                    DeleteDate = x.DeleteDate,
                }).ToList();
        }
    }
}
