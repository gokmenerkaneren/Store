using Store.Location.Application.Features.CQRS.Commands.WarehouseCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.WarehouseHandlers
{
    public class CreateWarehouseCommandHandler
    {
        private readonly IRepository<Warehouse> _repository;
        private readonly IRepository<WarehouseAddress> _addressRepository;

        public CreateWarehouseCommandHandler(IRepository<Warehouse> repository, IRepository<WarehouseAddress> addressRepository)
        {
            _repository = repository;
            _addressRepository = addressRepository;
        }

        public async Task Handle(CreateWarehouseCommand command)
        {
            var name = (command.Name ?? "").Trim();
            var code = (command.Code ?? "").Trim();

            var duplicates = await _repository.GetAllByFilterAsync(x => x.BranchId == command.BranchId && (x.Name.ToLower() == name.ToLower() || x.Code.ToLower() == code.ToLower()));
            var existing = duplicates.FirstOrDefault();
            if (existing != null)
            {
                string fieldLabel = existing.Name.ToLower() == name.ToLower() ? "Depo Adı" : "Depo Kodu";
                string matchedValue = existing.Name.ToLower() == name.ToLower() ? name : code;
                string msg = Store.Location.Application.Common.DuplicateCheckerHelper.BuildDuplicateMessage(fieldLabel, matchedValue, existing.IsDeleted, existing.IsActive);
                throw new InvalidOperationException(msg);
            }

            if (command.IsDefault && command.BranchId != null)
            {
                var existingDefaults = await _repository.GetAllByFilterAsync(x => x.BranchId == command.BranchId && x.IsDefault);
                foreach (var existingDefault in existingDefaults)
                {
                    existingDefault.IsDefault = false;
                    await _repository.UpdateAsync(existingDefault);
                }
            }

            var warehouse = new Warehouse
            {
                Name = name,
                Code = code,
                IsDefault = command.IsDefault,
                BranchId = command.BranchId,
                ContactInfoId = command.ContactInfoId,
                Notes = command.Notes,
                IsActive = true,
                IsDeleted = false,
                InsertUserId = Guid.Parse("00000001-0001-0001-0001-000000000001"),
                InsertDate = BaseEntity.GetLocalTime()
            };

            await _repository.CreateAsync(warehouse);

            if (command.Address != null)
            {
                var address = new WarehouseAddress
                {
                    WarehouseId = warehouse.Id,
                    Title = command.Address.Title,
                    ContactFullName = command.Address.ContactFullName,
                    PhoneNumber = command.Address.PhoneNumber,
                    AddressDetail = command.Address.AddressDetail,
                    CountryId = command.Address.CountryId,
                    CityId = command.Address.CityId,
                    DistrictId = command.Address.DistrictId,
                    NeighborhoodId = command.Address.NeighborhoodId,
                    LoadingDockInstructions = command.Address.LoadingDockInstructions,
                    IsActive = true,
                    IsDeleted = false,
                    InsertUserId = Guid.Parse("00000001-0001-0001-0001-000000000001"),
                    InsertDate = BaseEntity.GetLocalTime()
                };
                await _addressRepository.CreateAsync(address);
            }
        }
    }
}
