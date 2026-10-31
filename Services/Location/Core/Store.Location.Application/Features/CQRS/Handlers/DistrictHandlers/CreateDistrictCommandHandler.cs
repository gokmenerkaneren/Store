using Store.Location.Application.Features.CQRS.Commands.DistrictCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.DistrictHandlers
{
    public class CreateDistrictCommandHandler
    {
        private readonly IRepository<District> _repository;

        public CreateDistrictCommandHandler(IRepository<District> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateDistrictCommand command)
        {
            var name = (command.DistrictName ?? "").Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("İlçe Adı boş bırakılamaz.");
            }

            var allDistricts = await _repository.GetAllAsync(includeDeleted: true);

            var existing = allDistricts.FirstOrDefault(d =>
                d.CityId == command.CityId && (
                    string.Equals((d.DistrictName ?? "").Trim(), name, StringComparison.CurrentCultureIgnoreCase) ||
                    string.Equals((d.DistrictName ?? "").Trim(), name, StringComparison.OrdinalIgnoreCase)
                )
            );

            if (existing != null)
            {
                string msg = Store.Location.Application.Common.DuplicateCheckerHelper.BuildDuplicateMessage("İlçe Adı", existing.DistrictName, existing.IsDeleted, existing.IsActive);
                throw new InvalidOperationException(msg);
            }

            var district = new District
            {
                DistrictName = name,
                CityId = command.CityId,
                Notes = command.Notes,
                IsActive = true,
                IsDeleted = false,
                InsertUserId = Guid.Parse("00000001-0001-0001-0001-000000000001"),
                InsertDate = BaseEntity.GetLocalTime()
            };

            await _repository.CreateAsync(district);
        }
    }
}
