using Store.Location.Application.Features.CQRS.Queries.WarehouseQueries;
using Store.Location.Application.Features.CQRS.Results.WarehouseResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.WarehouseHandlers
{
    public class GetWarehouseByIdQueryHandler
    {
        private readonly IRepository<Warehouse> _repository;
        private readonly IRepository<WarehouseAddress> _addressRepository;

        public GetWarehouseByIdQueryHandler(IRepository<Warehouse> repository, IRepository<WarehouseAddress> addressRepository)
        {
            _repository = repository;
            _addressRepository = addressRepository;
        }

        public async Task<GetWarehouseByIdQueryResult?> Handle(GetWarehouseByIdQuery query)
        {
            var values = await _repository.GetByFilterAsync(x => x.Id == query.Id);
            if (values == null) return null;

            var addresses = await _addressRepository.GetAllByFilterAsync(x => x.WarehouseId == query.Id);
            var address = addresses.FirstOrDefault();

            return new GetWarehouseByIdQueryResult
            {
                Id = values.Id,
                Name = values.Name,
                Code = values.Code,
                IsDefault = values.IsDefault,
                BranchId = values.BranchId,
                ContactInfoId = values.ContactInfoId,
                IsActive = values.IsActive,
                IsDeleted = values.IsDeleted,
                Notes = values.Notes,
                InsertDate = values.InsertDate,
                UpdateDate = values.UpdateDate,
                InsertUserId = values.InsertUserId,
                UpdateUserId = values.UpdateUserId,
                DeleteUserId = values.DeleteUserId,
                DeleteDate = values.DeleteDate,
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
        }
    }
}
