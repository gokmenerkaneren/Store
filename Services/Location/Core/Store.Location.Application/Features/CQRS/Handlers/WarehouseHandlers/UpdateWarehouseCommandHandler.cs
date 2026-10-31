using Store.Location.Application.Features.CQRS.Commands.WarehouseCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.WarehouseHandlers
{
    public class UpdateWarehouseCommandHandler
    {
        private readonly IRepository<Warehouse> _repository;
        private readonly IRepository<WarehouseAddress> _addressRepository;

        public UpdateWarehouseCommandHandler(IRepository<Warehouse> repository, IRepository<WarehouseAddress> addressRepository)
        {
            _repository = repository;
            _addressRepository = addressRepository;
        }

        public async Task Handle(UpdateWarehouseCommand command)
        {
            var name = (command.Name ?? "").Trim();
            var code = (command.Code ?? "").Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("Depo Adı boş bırakılamaz.");
            }

            var all = await _repository.GetAllAsync(includeDeleted: true);

            var existing = all.FirstOrDefault(w =>
                w.Id != command.Id && w.BranchId == command.BranchId && (
                    (!string.IsNullOrWhiteSpace(name) && string.Equals((w.Name ?? "").Trim(), name, StringComparison.CurrentCultureIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(name) && string.Equals((w.Name ?? "").Trim(), name, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(code) && !string.IsNullOrWhiteSpace(w.Code) && (string.Equals((w.Code ?? "").Trim(), code, StringComparison.CurrentCultureIgnoreCase) || string.Equals((w.Code ?? "").Trim(), code, StringComparison.OrdinalIgnoreCase)))
                )
            );

            if (existing != null)
            {
                bool nameMatched = string.Equals((existing.Name ?? "").Trim(), name, StringComparison.CurrentCultureIgnoreCase) || string.Equals((existing.Name ?? "").Trim(), name, StringComparison.OrdinalIgnoreCase);
                string fieldLabel = nameMatched ? "Depo Adı" : "Depo Kodu";
                string matchedValue = nameMatched ? existing.Name : (existing.Code ?? "");

                string msg = Store.Location.Application.Common.DuplicateCheckerHelper.BuildDuplicateMessage(fieldLabel, matchedValue, existing.IsDeleted, existing.IsActive);
                throw new InvalidOperationException(msg);
            }

            if (command.IsDefault && command.BranchId != null)
            {
                var existingDefaults = all.Where(x => x.Id != command.Id && x.BranchId == command.BranchId && x.IsDefault).ToList();
                foreach (var existingDefault in existingDefaults)
                {
                    existingDefault.IsDefault = false;
                    await _repository.UpdateAsync(existingDefault);
                }
            }

            if (command.IsDefault && !command.IsActive)
            {
                throw new InvalidOperationException("Varsayılan depo pasife alınamaz.");
            }

            var warehouse = await _repository.GetByIdAsync(command.Id);
            warehouse.Name = name;
            warehouse.Code = code;
            warehouse.IsDefault = command.IsDefault;
            warehouse.BranchId = command.BranchId;
            warehouse.ContactInfoId = command.ContactInfoId;
            warehouse.Notes = command.Notes;
            warehouse.IsActive = command.IsActive;

            await _repository.UpdateAsync(warehouse);

            if (command.Address != null)
            {
                var existingAddresses = await _addressRepository.GetAllByFilterAsync(x => x.WarehouseId == warehouse.Id);
                var existingAddress = existingAddresses.FirstOrDefault();
                if (existingAddress != null)
                {
                    existingAddress.Title = command.Address.Title;
                    existingAddress.ContactFullName = command.Address.ContactFullName;
                    existingAddress.PhoneNumber = command.Address.PhoneNumber;
                    existingAddress.AddressDetail = command.Address.AddressDetail;
                    existingAddress.CountryId = command.Address.CountryId;
                    existingAddress.CityId = command.Address.CityId;
                    existingAddress.DistrictId = command.Address.DistrictId;
                    existingAddress.NeighborhoodId = command.Address.NeighborhoodId;
                    existingAddress.LoadingDockInstructions = command.Address.LoadingDockInstructions;
                    
                    await _addressRepository.UpdateAsync(existingAddress);
                }
                else
                {
                    var newAddress = new WarehouseAddress
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
                    await _addressRepository.CreateAsync(newAddress);
                }
            }
        }
    }
}
