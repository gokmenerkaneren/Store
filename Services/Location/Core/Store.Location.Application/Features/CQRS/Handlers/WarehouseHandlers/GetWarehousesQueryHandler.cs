using Store.Location.Application.Features.CQRS.Queries.WarehouseQueries;
using Store.Location.Application.Features.CQRS.Results.WarehouseResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.WarehouseHandlers
{
    public class GetWarehousesQueryHandler
    {
        private readonly IRepository<Warehouse> _repository;
        private readonly IRepository<WarehouseAddress> _addressRepository;

        public GetWarehousesQueryHandler(IRepository<Warehouse> repository, IRepository<WarehouseAddress> addressRepository)
        {
            _repository = repository;
            _addressRepository = addressRepository;
        }

        public async Task<List<GetWarehouseQueryResult>> Handle(bool showDeleted = false)
        {
            var warehouses = await _repository.GetAllAsync(includeDeleted: showDeleted);
            if (!showDeleted) warehouses = warehouses.Where(x => !x.IsDeleted).ToList();
            
            var addresses = await _addressRepository.GetAllAsync(includeDeleted: showDeleted);

            return warehouses.Select(x => {
                var address = addresses.FirstOrDefault(a => a.WarehouseId == x.Id);
                return new GetWarehouseQueryResult
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                    IsDefault = x.IsDefault,
                    BranchId = x.BranchId,
                    ContactInfoId = x.ContactInfoId,
                    IsActive = x.IsActive,
                    Notes = x.Notes,
                    InsertUserId = x.InsertUserId,
                    InsertDate = x.InsertDate,
                    UpdateUserId = x.UpdateUserId,
                    UpdateDate = x.UpdateDate,
                    IsDeleted = x.IsDeleted,
                    DeleteUserId = x.DeleteUserId,
                    DeleteDate = x.DeleteDate,
                    Address = address != null ? new Store.DtoLayer.LocationDtos.WarehouseDtos.WarehouseAddressDto
                    {
                        Id = address.Id,
                        Title = address.Title,
                        ContactFullName = address.ContactFullName,
                        PhoneNumber = address.PhoneNumber,
                        AddressDetail = address.AddressDetail,
                        CountryId = address.CountryId,
                        CityId = address.CityId,
                        DistrictId = address.DistrictId,
                        NeighborhoodId = address.NeighborhoodId,
                        LoadingDockInstructions = address.LoadingDockInstructions
                    } : null
                };
            }).ToList();
        }
    }
}
