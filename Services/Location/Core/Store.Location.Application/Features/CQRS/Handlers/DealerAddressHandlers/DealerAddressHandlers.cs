using Store.Location.Application.Features.CQRS.Commands.DealerAddressCommands;
using Store.Shared.Enums;
using Store.Location.Application.Features.CQRS.Queries.DealerAddressQueries;
using Store.Shared.Enums;
using Store.Location.Application.Features.CQRS.Results.DealerAddressResults;
using Store.Shared.Enums;
using Store.Location.Application.Interfaces;
using Store.Shared.Enums;
using Store.Location.Domain.Entities;
using Store.Shared.Enums;

namespace Store.Location.Application.Features.CQRS.Handlers.DealerAddressHandlers
{
    public class CreateDealerAddressCommandHandler
    {
        private readonly IRepository<DealerAddress> _repository;

        public CreateDealerAddressCommandHandler(IRepository<DealerAddress> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateDealerAddressCommand command)
        {
            var dealerAddress = new DealerAddress
            {
                DealerId = command.DealerId,
                AddressType = command.AddressType,
                Title = command.Title,
                ContactFullName = command.ContactFullName,
                PhoneNumber = command.PhoneNumber,
                AddressDetail = command.AddressDetail,
                CountryId = command.CountryId,
                CityId = command.CityId,
                DistrictId = command.DistrictId,
                NeighborhoodId = command.NeighborhoodId,
                Notes = command.Notes,
                IsActive = true,
                IsDeleted = false,
                InsertUserId = Guid.Parse("00000001-0001-0001-0001-000000000001"),
                InsertDate = BaseEntity.GetLocalTime()
            };

            await _repository.CreateAsync(dealerAddress);
        }
    }

    public class UpdateDealerAddressCommandHandler
    {
        private readonly IRepository<DealerAddress> _repository;

        public UpdateDealerAddressCommandHandler(IRepository<DealerAddress> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateDealerAddressCommand command)
        {
            var dealerAddress = await _repository.GetByFilterAsync(x => x.Id == command.Id);
            if (dealerAddress == null) return;

            dealerAddress.DealerId = command.DealerId;
            dealerAddress.AddressType = command.AddressType;
            dealerAddress.Title = command.Title;
            dealerAddress.ContactFullName = command.ContactFullName;
            dealerAddress.PhoneNumber = command.PhoneNumber;
            dealerAddress.AddressDetail = command.AddressDetail;
            dealerAddress.CountryId = command.CountryId;
            dealerAddress.CityId = command.CityId;
            dealerAddress.DistrictId = command.DistrictId;
            dealerAddress.NeighborhoodId = command.NeighborhoodId;
            dealerAddress.IsActive = command.IsActive;
            dealerAddress.Notes = command.Notes;
            dealerAddress.UpdateUserId = Guid.Parse("00000002-0002-0002-0000-000000000002");
            dealerAddress.UpdateDate = BaseEntity.GetLocalTime();

            await _repository.UpdateAsync(dealerAddress);
        }
    }

    public class GetDealerAddressByIdQueryHandler
    {
        private readonly IRepository<DealerAddress> _repository;

        public GetDealerAddressByIdQueryHandler(IRepository<DealerAddress> repository)
        {
            _repository = repository;
        }

        public async Task<GetDealerAddressByIdQueryResult?> Handle(GetDealerAddressByIdQuery query)
        {
            var values = await _repository.GetByFilterAsync(x => x.Id == query.Id);
            if (values == null) return null;

            return new GetDealerAddressByIdQueryResult
            {
                Id = values.Id,
                DealerId = values.DealerId,
                AddressType = values.AddressType,
                Title = values.Title,
                ContactFullName = values.ContactFullName,
                PhoneNumber = values.PhoneNumber,
                AddressDetail = values.AddressDetail,
                CountryId = values.CountryId,
                CityId = values.CityId,
                DistrictId = values.DistrictId,
                NeighborhoodId = values.NeighborhoodId,
                CountryName = values.Country?.CountryName,
                CityName = values.City?.CityName,
                DistrictName = values.District?.DistrictName,
                NeighborhoodName = values.Neighborhood?.NeighborhoodName,
                IsActive = values.IsActive,
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

    public class GetDealerAddressesByDealerIdQueryHandler
    {
        private readonly IRepository<DealerAddress> _repository;

        public GetDealerAddressesByDealerIdQueryHandler(IRepository<DealerAddress> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetDealerAddressQueryResult>> Handle(GetDealerAddressesByDealerIdQuery query)
        {
            var addresses = await _repository.GetAllByFilterAsync(x => x.DealerId == query.DealerId);
            return addresses.Select(x => new GetDealerAddressQueryResult
            {
                Id = x.Id,
                DealerId = x.DealerId,
                AddressType = x.AddressType,
                Title = x.Title,
                ContactFullName = x.ContactFullName,
                PhoneNumber = x.PhoneNumber,
                AddressDetail = x.AddressDetail,
                CountryId = x.CountryId,
                CityId = x.CityId,
                DistrictId = x.DistrictId,
                NeighborhoodId = x.NeighborhoodId,
                CountryName = x.Country?.CountryName,
                CityName = x.City?.CityName,
                DistrictName = x.District?.DistrictName,
                NeighborhoodName = x.Neighborhood?.NeighborhoodName,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted,
                Notes = x.Notes,
                InsertDate = x.InsertDate,
                UpdateDate = x.UpdateDate,
                InsertUserId = x.InsertUserId,
                UpdateUserId = x.UpdateUserId,
                DeleteUserId = x.DeleteUserId,
                DeleteDate = x.DeleteDate,
            }).ToList();
        }
    }

    public class DeleteDealerAddressCommandHandler
    {
        private readonly IRepository<DealerAddress> _repository;

        public DeleteDealerAddressCommandHandler(IRepository<DealerAddress> repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteDealerAddressCommand command)
        {
            var dealerAddress = await _repository.GetByIdAsync(command.Id);
            if (dealerAddress != null)
            {
                await _repository.SoftDeleteAsync(dealerAddress);
            }
        }
    }

    public class SoftDeleteDealerAddressCommandHandler
    {
        private readonly IRepository<DealerAddress> _repository;

        public SoftDeleteDealerAddressCommandHandler(IRepository<DealerAddress> repository)
        {
            _repository = repository;
        }

        public async Task Handle(SoftDeleteDealerAddressCommand command)
        {
            var dealerAddress = await _repository.GetByFilterAsync(x => x.Id == command.Id);
            if (dealerAddress != null)
            {
                await _repository.SoftDeleteAsync(dealerAddress);
            }
        }
    }

    public class RestoreDealerAddressCommandHandler
    {
        private readonly IRepository<DealerAddress> _repository;

        public RestoreDealerAddressCommandHandler(IRepository<DealerAddress> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RestoreDealerAddressCommand command)
        {
            var dealerAddress = await _repository.GetByFilterAsync(x => x.Id == command.Id);
            if (dealerAddress != null)
            {
                await _repository.RestoreAsync(dealerAddress);
            }
        }
    }
}
