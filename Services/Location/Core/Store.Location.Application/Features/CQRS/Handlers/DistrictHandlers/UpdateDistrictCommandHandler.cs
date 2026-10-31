using Store.Location.Application.Features.CQRS.Commands.DistrictCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.DistrictHandlers
{
    public class UpdateDistrictCommandHandler
    {
        private readonly IRepository<District> _repository;

        public UpdateDistrictCommandHandler(IRepository<District> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateDistrictCommand command)
        {
            var name = (command.DistrictName ?? "").Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("İlçe Adı boş bırakılamaz.");
            }

            var allDistricts = await _repository.GetAllAsync(includeDeleted: true);

            var existing = allDistricts.FirstOrDefault(d =>
                d.Id != command.Id && d.CityId == command.CityId && (
                    string.Equals((d.DistrictName ?? "").Trim(), name, StringComparison.CurrentCultureIgnoreCase) ||
                    string.Equals((d.DistrictName ?? "").Trim(), name, StringComparison.OrdinalIgnoreCase)
                )
            );

            if (existing != null)
            {
                string msg = Store.Location.Application.Common.DuplicateCheckerHelper.BuildDuplicateMessage("İlçe Adı", existing.DistrictName, existing.IsDeleted, existing.IsActive);
                throw new InvalidOperationException(msg);
            }

            var district = await _repository.GetByIdAsync(command.Id);
            district.DistrictName = name;
            district.CityId = command.CityId;
            district.Notes = command.Notes;
            district.IsActive = command.IsActive;

            await _repository.UpdateAsync(district);
        }
    }
}
